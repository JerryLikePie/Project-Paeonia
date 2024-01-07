using System.Collections;
using System.Collections.Generic;
using static Utilities;
using UnityEngine;

// 轰炸机
public class BomberCombatBehavior : IDollsCombatBehaviour
{
    Vector3 airBase = new Vector3(-700, 80, 100);
    Vector3 exitPoint = new Vector3(500, 80, 100);
    private Vector3 height = new Vector3(0, 1, 0);
    private Vector3 takeoffdir = new Vector3(0, 90, 0);
    private DollsCombat context;
    private Queue<Hex> toCancelFog;
    public float airSpeed, fireInterval, gunInterval;
    public bool canAttack, useGun;
    public AudioSource bombdrop, gunning, engineNear;
    public GameObject getTarget;
    private Vector3 target;
    GameObject setEnemy;
    // 设定状态：锁敌，RTB，改出，油量
    bool targetLocked, returning, recovering, liftoff;
    public bool isDone;

    // The maximum angle of rotation for pitch and roll
    public float maxAngle;

    // The rigidbody component of the airplane
    private Rigidbody rb;

    void Start()
    {
        toCancelFog = new Queue<Hex>();
        transform.position = airBase;

        rb = GetComponent<Rigidbody>();
        // Disable the gravity
        rb.useGravity = false;
        //animator.Play("Flying");

        setEnemy = null;
        targetLocked = false;
        recovering = false;
        returning = false;
        liftoff = false;
        isDone = false;
    }
    public override void WreckAfterDead(DollsCombat context, Transform deadbody)
    {
        // 生成残骸
        if (context.wreckage == null)
        {
            // 没有绑定残骸吗？那就不生成了
            Debug.Log("No wreckage?");
            return;
        }
        GameObject body = Instantiate(context.wreckage, deadbody.position, Quaternion.identity);
        body.SetActive(true);
        // 把当前的motion vector给到残骸上面，残骸沿着这个方向抛物前进
        Vector3 planefwd = context.gunCenter.transform.position - context.transform.position;
        planefwd = planefwd.normalized;
        planefwd = planefwd * airSpeed;
        //body.GetComponent<Rigidbody>().AddRelativeForce(planefwd);
        Debug.Log(planefwd);
        body.GetComponent<Rigidbody>().velocity = planefwd;
    }
    public override void CheckEnemy(DollsCombat context)
    {
        // 按帧索敌
        // Don't crash
        if (transform.position.y <= 0)
        {
            Debug.LogError("you crashed");
            // placeholder
        }
        // If out of ammo rtb
        if (context.outofAmmo)
        {
            returning = true;
        }

        // Check sprite direction
        flipSpriteOnDirection(context);
        if (context.supportTargetCord != null && canAttack)
        {
            // bombing run
            context.thisUnit.engineSound.volume = 1f;
            engineNear.volume = 1f;
            //SetTarget(context.supportTargetCord.position, context);
            GroundStrike(context);
            AirRecon(context, 2);
            if (!liftoff)
            {
                liftoff = true;
            }
        }
        else
        {
            context.thisUnit.engineSound.volume = 0f;
            engineNear.volume = 0f;
            transform.position = airBase;
            transform.eulerAngles = takeoffdir;
            if (liftoff)
            {
                liftoff = false;
            }
        }

    }

    void SetTarget(Vector3 center, DollsCombat context)
    {
        // 不需要知道敌人位置
        // only set getTarget to the designated place
        //getTarget.transform.position = center;
    }

    void GroundStrike(DollsCombat context)
    {
        // Depending on the distance to the target, change the behavior
        if (returning)
        {
            returningToBase(context);
        }
        else
        {
            Vector3 tempTarget = context.supportTargetCord.position + 80 * height;
            float distance = FindDistance(tempTarget, transform.position);
            if (distance > CalculateLaunch() & !recovering)
            {
                // 计算炸弹落点
                target = tempTarget;
            }
            else if (distance > 0 & !recovering)
            {
                // 投弹，开始改出
                DropBombs(context);
                recovering = true;
                target = getRecoveryTarget();
                StartCoroutine(Recovered(airSpeed / 10f));
            }
            else if (recovering)
            {
                //Debug.Log("改出");
                target = getRecoveryTarget();
            }
        }
        moveToTarget();
    }

    void DropBombs(DollsCombat context)
    {
        if (context.canFire)
        {
            context.planeVelocity = airSpeed * 0.95f;
            context.counter = 0;
            context.Attack();
            //Debug.Log(gunning.clip.length);
            StartCoroutine(context.FireRate(fireInterval, false));
            if (!bombdrop.isPlaying)
            {
                bombdrop.Play();
            }

        }
    }

    float CalculateLaunch()
    {
        float t = Mathf.Sqrt(2 * transform.position.y / 9.8f);
        float dist = t * airSpeed;
        return dist;
    }

    Vector3 getRecoveryTarget()
    {
        // 改出航线为向前改出
        Vector3 newPos = transform.position + 150 * transform.forward;
        newPos.y = 80;
        return newPos;
    }

    Vector3 getAvoidCollision(Vector3 agianst)
    {
        // 改出航线为向侧方向改出
        Vector3 newPos = agianst + 150 * Vector3.right;
        newPos.y = 70;
        return newPos;
    }

    public bool taskDone()
    {
        return isDone;
    }

    void returningToBase(DollsCombat context)
    {
        // rtb
        target = airBase;
        float distance = FindDistance(airBase, transform.position);
        if (distance < 40f)
        {
            // 到达机场
            context.supportTargetCord = null;
            isDone = true;
            returning = false;
            StopCoroutine("RTB");
            // Reload
            StartCoroutine(context.FireRate(fireInterval, true));
        }
    }

    void moveToTarget()
    {
        transform.position += transform.forward * airSpeed * Time.deltaTime;
        Vector3 direction = (target - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, direction);
        float distance = FindDistance(target, transform.position);

        // If the angle is not zero, rotate towards the direction vector 
        if (angle != 0)
        {
            // Calculate the cross product of the forward vector and the direction vector 
            Vector3 cross = Vector3.Cross(transform.forward, direction);
            var rotate = Quaternion.LookRotation(target - transform.position);
            float t = Time.deltaTime / (angle / maxAngle);
            //Debug.Log(maxAngle * Time.deltaTime);
            // 现在就这样吧，差不多是linear，之后再想怎么smooth turn
            // 说实话现在这样也够用了


            transform.rotation = Quaternion.Lerp(transform.rotation, rotate, t);

            if ((angle / maxAngle) > (distance / airSpeed))
            {
                //If we can't make the turn, we'll just be turning in circles, so we move further to make the turn
                //transform.rotation = Quaternion.Lerp(transform.rotation, rotate, t);
                if (!recovering)
                {
                    recovering = true;
                    target = getRecoveryTarget();
                    StartCoroutine(Recovered(airSpeed / 15f));
                }
            }
            else
            {

            }

        }
    }

    public void newTask()
    {
        setEnemy = null;
        targetLocked = false;
        recovering = false;
        returning = false;
        liftoff = false;
        isDone = false;
        // max air time 60 seconds
        StartCoroutine("RTB");
    }

    void flipSpriteOnDirection(DollsCombat context)
    {
        // Flip sprite
        for (int i = 0; i < context.crewNum; i++)
        {
            context.dollsEntities[i].flip(isGoingRight());
        }
    }

    bool isGoingRight()
    {
        // 用点乘测得飞机相对于摄像机视角是否在向右飞行
        Vector3 forward = transform.forward;
        Vector3 camRight = Camera.main.transform.right;
        float relative = Vector3.Dot(forward, camRight);
        if (relative > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void AirRecon(DollsCombat context, int increaseRange)
    {
        Hex NextTile;
        for (int i = 0; i <= context.map.transform.childCount - 1; i++)
        {
            try
            {
                NextTile = context.map.transform.GetChild(i).GetComponent<Hex>();
                if (FindDistance(gameObject, NextTile.gameObject) <= 17.5 * (context.dolls.dolls_view_range + increaseRange) && NextTile.isInFog == 0)
                {
                    if (!NextTile.blockVision)
                    {
                        NextTile.GainVisual();
                        toCancelFog.Enqueue(NextTile);
                        Invoke("DeFogOfWarOneAtATime", 10f);
                    }
                    else
                    {
                        NextTile.render = true;
                        toCancelFog.Enqueue(NextTile);
                        Invoke("DeFogOfWarOneAtATime", 10f);
                    }
                }
                NextTile.UpdateFogStatus();

            }
            catch
            {
                continue;
            }
        }
    }
    void DeFogOfWarOneAtATime()
    {
        try
        {
            Hex hex = toCancelFog.Dequeue();
            hex.LoseVisual();
            hex.UpdateFogStatus();
        }
        catch
        {
        }
    }

    public IEnumerator Recovered(float time)
    {
        yield return new WaitForSeconds(time);
        recovering = false;
    }

    public IEnumerator RTB()
    {
        yield return new WaitForSeconds(60);
        //Debug.LogError("Returning");
        returning = true;
        targetLocked = false;
    }
}