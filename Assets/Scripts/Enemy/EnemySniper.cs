using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utilities;

public class EnemySniper : IEnemyBehavior
{
    // 敌方的坦歼

    private EnemyCombat context;
    private DollsCombat doll;
    int newRange;


    public override void CheckDolls(EnemyCombat context)
    {
        
        if (context.canFire)
        {
            try
            {
                if (context.bullet == null)
                {
                    // 子弹没准备好的话应该是出错了
                    Debug.LogError("没有子弹");
                    return;
                }
                if (context.rangeBuff > 0)
                {
                    newRange = (int)context.rangeBuff + context.enemy.enemy_range;
                }
                else
                {
                    newRange = context.enemy.enemy_range;
                }
                for (int i = 0; i < context.dollsList.transform.childCount; i++)
                {
                    doll = context.dollsList.transform.GetChild(i).GetComponent<DollsCombat>();
                    if (doll == null)
                    {
                        continue;
                    }
                    if (doll.getType() == 3)
                    {
                        // 如果是空军的话无视
                        continue;
                    }
                    if (FindDistance(transform.gameObject, doll.gameObject) <= 17.5 * newRange)
                    {
                        if (doll.beingSpotted && doll.gameObject.activeSelf)
                        {
                            attack(context);
                        }

                    }
                }
            }
            catch
            {
                Debug.LogError("无法识别到单位！！");
            }
        }
    }

    void attack(EnemyCombat context)
    {
        context.counter = 0;
        context.setDolls = doll;
        for (int j = 0; j < context.crewNum; j++)
        {
            context.Invoke("FireBullet", Random.Range(0, 0.34f));
        }
        StartCoroutine(context.FireRate());
    }
}
