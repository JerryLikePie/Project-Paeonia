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
    public float airSpeed;
    private long timenow;
    private float time;
    public bool canAttack;
    public AudioSource gunning;
    public GameObject getTarget;
    private Vector3 target;
    GameObject setEnemy;
    bool targetLocked, returning, recovering;


    // The maximum angle of rotation for pitch and roll
    public float maxAngle;

    // The rigidbody component of the airplane
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        toCancelFog = new Queue<Hex>();
        transform.position = airBase;
        transform.Rotate(0, 90, 0);

        rb = GetComponent<Rigidbody>();
        // Disable the gravity
        rb.useGravity = false;
        //animator.Play("Flying");

        setEnemy = null;
        targetLocked = false;
        recovering = false;
    }

    public override void CheckEnemy(DollsCombat context)
    {
        flipSpriteOnDirection(context);
        if (context.outofAmmo)
        {
            Debug.Log("We are out of ammo!");
        }
        if (context.supportTargetCord != null && canAttack)
        {
            context.thisUnit.engineSound.volume = 1f;
            SetTarget(context.supportTargetCord.position, context);
            //Debug.Log(target);
            GroundStrike(context);
            AirRecon(context, 1);
        }
        else if (context.supportTargetCord != null && !canAttack)
        {
            //context.thisUnit.engineSound.volume = 1f;
            //GroundStrike(context.supportTargetCord.position);
            //AirRecon(context, 1);
        }
        else
        {
            context.thisUnit.engineSound.volume = 0f;
            transform.position = airBase;
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
                if (distance <= 17.5)
                {
                    setEnemy = context.enemyList[i].gameObject;
                    context.setEnemy = setEnemy.GetComponent<EnemyCombat>();
                    targetLocked = true;
                    break;
                }
            }
        }
    }
    

    void GroundStrike(DollsCombat context)
    {
        // Depending on the distance to the target, change the behavior
        if (setEnemy.activeSelf == false)
        {
            targetLocked = false;
            target = airBase;
        }
        else
        {
            // 有敌人
            float distance = FindDistance(setEnemy, transform.gameObject);
            if (distance > 300 & !recovering)
            {
                //Debug.Log("向敌人头上飞");
                target = setEnemy.transform.position + 50 * height;
            }
            else if (distance > 110 & !recovering)
            {
                //Debug.Log("浅俯冲");
                target = setEnemy.transform.position - 0 * height;
            }
            else if (distance > 65 & !recovering)
            {   //正对目标
                target = setEnemy.transform.position;
            }
            else if (distance > 50 & !recovering)
            {
                //Debug.Log("开始改出");
                target = setEnemy.transform.position + 100 * height;
            }
            else if (distance > 0 & !recovering)
            {
                recovering = true;
                target = getRecoveryTarget();
                StartCoroutine(Recovered(6));
            }
            else
            {
                recovering = true;
                //Debug.Log("改出");
                target = getRecoveryTarget();
            }
        }

        // Move forward at constant speed
        transform.position += transform.forward * airSpeed * Time.deltaTime;

        // Calculate the direction vector to the target location
        Vector3 direction = (target - transform.position).normalized;

        // Calculate the angle between the forward vector and the direction vector
        float angle = Vector3.Angle(transform.forward, direction);

        // If the angle is not zero, rotate towards the direction vector 
        if (angle != 0)
        {
            // Calculate the cross product of the forward vector and the direction vector 
            Vector3 cross = Vector3.Cross(transform.forward, direction);
            var rotate = Quaternion.LookRotation(target - transform.position);
            //transform.rotation = Quaternion.Slerp(transform.rotation, rotate, maxAngle);
            transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, rotate, maxAngle * Time.deltaTime);

        }

        AttackObjects(context);
    }

    Vector3 getRecoveryTarget()
    {
        Vector3 newPos = transform.position + 150 * transform.forward;
        newPos.y = 60;
        return newPos;
    }

    void flipSpriteOnDirection(DollsCombat context)
    {
        for (int i = 0; i < context.crewNum; i++)
        {
            context.dollsEntities[i].flip(isGoingRight());
        }
    }

    bool isGoingRight()
    {
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
        RaycastHit hit;
        int layerMask = 1 << 11;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 180f, layerMask))
        {
            if (hit.collider.gameObject.tag == "Enemy")
            {
                //Debug.Log("An object has been attacked: " + hit.collider.gameObject.name);
                if (context.canFire)
                {
                    context.planeVelocity = transform.forward * airSpeed * Time.deltaTime * 60;
                    context.counter = 0;
                    context.Strafe();
                    StartCoroutine(context.FireRate(gunning.clip.length, false));

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

    public IEnumerator Recovered(int time)
    {
        yield return new WaitForSeconds(time);
        recovering = false;
    }

}
