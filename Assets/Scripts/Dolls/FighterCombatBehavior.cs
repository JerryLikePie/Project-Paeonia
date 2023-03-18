using System.Collections;
using System.Collections.Generic;
using static Utilities;
using UnityEngine;

public class FighterCombatBehavior : IDollsCombatBehaviour
{

    private Queue<Hex> toCancelFog;
    Vector3 airBase = new Vector3(-500, 50, 100);
    Vector3 exitPoint = new Vector3(500, 80, 100);
    private Vector3 height = new Vector3(0, 1, 0);
    private Vector3 takeoffdir = new Vector3(0, 90, 0);
    public float airSpeed, fireInterval;
    private long timenow;
    private float time;
    // if bombingRun == false -> guns
    public bool canAttack, bombingRun;
    public AudioSource gunning;
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
            // attack ground
            context.thisUnit.engineSound.volume = 1f;
            SetTarget(context.supportTargetCord.position, context);
            GroundStrike(context);
            AirRecon(context, 1);
            if (!liftoff)
            {
                liftoff = true;
            }
        }
        else if (context.supportTargetCord != null && !canAttack)
        {
            // attack air
            // does not recon
            context.thisUnit.engineSound.volume = 1f;
            SetTarget(context.supportTargetCord.position, context);
            AirSuppress(context);
            if (!liftoff)
            {
                target = context.supportTargetCord.position + 60 * height;
                liftoff = true;
            }
        }
        else
        {
            context.thisUnit.engineSound.volume = 0f;
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
        getTarget.transform.position = center;
        
        if (targetLocked)
        {
            return;
        }
        for (int i = 0; i < context.enemyList.Count; i++)
        {
            if (!context.enemyList[i].gameObject.activeSelf)
            {
                context.enemyList.RemoveAt(i);
                i = 0;
                continue;
            }
            if (context.enemyList[i].gameObject.activeSelf)
            {
                float distance = FindDistance(getTarget, context.enemyList[i].gameObject);
                if (canAttack)
                {
                    // look for ground targets at the designated place
                    // 舔地，当然顺便制空也是可以的
                    if (distance <= 17.5)
                    {
                        setEnemy = context.enemyList[i].gameObject;
                        context.setEnemy = setEnemy.GetComponent<EnemyCombat>();
                        targetLocked = true;
                        break;
                    }
                }
                else
                {
                    // look for air targets above the battlefield
                    // 制空
                    if (context.enemyList[i].transform.position.y > 15)
                    {
                        setEnemy = context.enemyList[i].gameObject;
                        context.setEnemy = setEnemy.GetComponent<EnemyCombat>();
                        targetLocked = true;
                        break;
                    }
                }
                
            }
        }
    }

    void AirSuppress(DollsCombat context)
    {
        if (returning)
        {
            returningToBase(context);
        }
        else
        {
            if (setEnemy == null && !returning)
            {
                // 转圈圈
                float distance = FindDistance(target, transform.position);
                if (distance < 40f)
                {
                    int randX = Random.Range(0, 200);
                    if (randX < 100)
                    {
                        randX = -(100 + randX);
                    }
                    int randZ = Random.Range(0, 200);
                    if (randZ < 100)
                    {
                        randZ = -(100 + randZ);
                    }
                    Vector3 rand = new Vector3(randX, 60, randZ);
                    target = context.supportTargetCord.position + rand;
                }
            }
            else if (setEnemy.activeSelf == false)
            {
                // there is no enemy
                targetLocked = false;
                float distance = FindDistance(context.supportTargetCord.position, transform.position);
                if (!recovering)
                {
                    target = context.supportTargetCord.position + 60 * height;
                    if (distance < 30f)
                    {
                        recovering = true;
                        target = getRecoveryTarget();
                        StartCoroutine(Recovered(airSpeed / 10f));
                    }
                }
                else
                {
                    //Debug.Log("改出");
                    target = getRecoveryTarget();
                }
            }
            else
            {
                // has enemy
                float distance = FindDistance(setEnemy, transform.gameObject);
                // making sure don't crush into them
                if (distance > airSpeed * 0.9f && !recovering)
                {
                    
                    target = setEnemy.transform.position;
                }
                else if (distance > 0 & !recovering)
                {
                    //改出
                    recovering = true;
                    StartCoroutine(AvoidColli(airSpeed / 15f));
                }
                else
                {
                    recovering = true;
                }
            }
        }
        moveToTarget();
        AttackObjects(context);
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
            if (setEnemy == null && !returning)
            {
                target = context.supportTargetCord.position + 60 * height;
                float distance = FindDistance(target, transform.position);
                
                //Debug.Log(distance);
                if (distance < 5f)
                {
                    returning = true;
                }
            }
            else if (setEnemy.activeSelf == false)
            {
                // there is no enemy
                // Debug.Log("There is no enemy");
                canAttack = false;
                AirSuppress(context);
                return;
                targetLocked = false;
                float distance = FindDistance(context.supportTargetCord.position, transform.position);
                if (!recovering)
                {
                    target = context.supportTargetCord.position + 60 * height;
                    if (distance < 30f)
                    {
                        recovering = true;
                        target = getRecoveryTarget();
                        StartCoroutine(Recovered(airSpeed / 10f));
                    }
                }
                else
                {
                    //Debug.Log("改出");
                    target = getRecoveryTarget();
                }

            }
            else
            {
                // Has Enemy
                float distance = FindDistance(setEnemy, transform.gameObject);
                if (distance > airSpeed * 4 & !recovering)
                {
                    //Debug.Log("向敌人头上飞");
                    target = setEnemy.transform.position + 50 * height;
                }
                else if (distance > airSpeed * 1.5f & !recovering)
                {
                    //Debug.Log("浅俯冲");
                    target = setEnemy.transform.position - 0 * height;
                }
                else if (distance > airSpeed & !recovering)
                {   //正对目标
                    target = setEnemy.transform.position;
                }
                else if (distance > airSpeed * 0.75f & !recovering)
                {
                    //Debug.Log("开始改出");
                    target = setEnemy.transform.position + 100 * height;
                }
                else if (distance > 0 & !recovering)
                {
                    recovering = true;
                    target = getRecoveryTarget();
                    StartCoroutine(Recovered(airSpeed / 10f));
                }
                else
                {
                    //Debug.Log("改出");
                    target = getRecoveryTarget();
                }
            }
        }
        moveToTarget();
        AttackObjects(context);
    }

    Vector3 getRecoveryTarget()
    {
        // 改出航线为向前改出
        Vector3 newPos = transform.position + 150 * transform.forward;
        newPos.y = 60;
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
            float t = Time.deltaTime/ (angle / maxAngle);
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

    void AttackObjects(DollsCombat context)
    {
        // Attack Ground targets
        RaycastHit hit;
        // Set layermask to 11 which is the enemy layer
        int layerMask = 1 << 11;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 2.5f * airSpeed, layerMask))
        {
            if (hit.collider.gameObject.tag == "Enemy")
            {
                //Debug.Log("An object has been attacked: " + hit.collider.gameObject.name);
                if (context.canFire)
                {
                    context.planeVelocity = transform.forward * airSpeed * Time.deltaTime * 60;
                    context.counter = 0;
                    context.Strafe();
                    //Debug.Log(gunning.clip.length);
                    StartCoroutine(context.FireRate(fireInterval, false));

                }
            }
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
                if (FindDistance(gameObject.transform.position, NextTile.gameObject.transform.position) <= 17.5 * (context.dolls.dolls_view_range + increaseRange) && NextTile.isInFog == 0)
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

    public IEnumerator AvoidColli(float time)
    {
        target = transform.right * 5000;
        target.y = 60;
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
