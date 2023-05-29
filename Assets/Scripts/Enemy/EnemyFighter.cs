using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utilities;

public class EnemyFighter : IEnemyBehavior
{
    private DollsCombat doll;
    public override void CheckDolls(EnemyCombat context)
    {
        try
        {
            if (transform.position.y <= 0)
            {
                context.RecieveExplosiveDamage(context.health);
            }
            bool inRange = false;

            for (int i = 0; i <= context.dollsList.transform.childCount - 1; i++)
            {
                if (context.isTarget)
                {
                    continue;
                }
                doll = context.dollsList.transform.GetChild(i).GetComponent<DollsCombat>();
                if (!doll.gameObject.activeSelf)
                {
                    continue;
                }
                if (doll.getType() != 3)
                {
                    continue;
                }
                if (FindDistance(transform.position, context.startPos) > 250)
                {
                    continue;
                }
                if (FindDistance(transform.gameObject, doll.gameObject) <= 17.5 * (context.enemy.enemy_range + 20))
                {
                    inRange = true;
                    context.target = doll.transform.position;
                }
            }
            if (!inRange)
            {
                context.target = context.startPos;
            }
        }
        catch
        {
        }
        for (int j = 0; j < context.crewNum; j++)
        {
            context.enemyEntities[j].flip(isGoingLeft());
        }
        moveToTarget(context);
        airAttack(context);
    }

    void airAttack(EnemyCombat context)
    {
        if (context.isTarget)
        {
            return;
        }
        RaycastHit hit;
        // Set layermask to 11 which is the friendly layer
        int layerMask = 1 << 10;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 150, layerMask))
        {
            if (hit.collider.gameObject.tag == "Friendly")
            {
                //Debug.Log("An object has been attacked: " + hit.collider.gameObject.name);
                if (context.canFire)
                {
                    context.counter = 0;
                    context.setDolls = doll;
                    for (int j = 0; j < context.crewNum; j++)
                    {
                        context.Invoke("Strafe", Random.Range(0, 0.05f));
                    }
                    StartCoroutine(context.FireRate());
                }
            }
        }
    }

    bool isGoingLeft()
    {
        // 用点乘测得飞机相对于摄像机视角是否在向右飞行
        Vector3 forward = transform.forward;
        Vector3 camRight = Camera.main.transform.right;
        float relative = Vector3.Dot(forward, camRight);
        if (relative < 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void moveToTarget(EnemyCombat context)
    {
        if (context.isTarget)
        {
            return;
        }
        //临时用着吧
        transform.position += transform.forward * 25 * Time.deltaTime;
        Vector3 direction = (context.target - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, direction);
        //float distance = FindDistance(target, transform.position);

        // If the angle is not zero, rotate towards the direction vector 
        if (angle != 0)
        {
            // Calculate the cross product of the forward vector and the direction vector 
            Vector3 cross = Vector3.Cross(transform.forward, direction);
            var rotate = Quaternion.LookRotation(context.target - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotate, 0.5f * Time.deltaTime);
        }
    }
}
