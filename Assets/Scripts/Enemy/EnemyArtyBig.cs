using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class EnemyArtyBig : IEnemyBehavior
{
    // 敌方的大口径火炮
    // 不主动攻击，对标定地点进行火炮攻击

    public override void CheckDolls(EnemyCombat context)
    {
        try
        {
            if (context.canFire)
            {
                GameObject maybeTarget = GameObject.FindWithTag("ArtilleryReference");
                context.supportTargetCord = maybeTarget.transform;
                attack(context);
            }
        }
        catch
        {
        }
    }

    void attack(EnemyCombat context)
    {
        context.counter = 0;
        for (int j = 0; j < context.crewNum; j++)
        {
            context.Invoke("FireHowitzer", Random.Range(0, 2));
        }
        StartCoroutine(context.FireRate());
    }
}
