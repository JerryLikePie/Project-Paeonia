using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utilities;

public class EnemyTanker : IEnemyBehavior
{
    private EnemyCombat context;
    private DollsCombat doll;
    int newRange;

    // Start is called before the first frame update
    void Start()
    {
    }

    public override void CheckDolls(EnemyCombat context)
    {
        if (context.canFire)
        {
            try
            {
                if (context.bullet == null)
                {
                    // 子弹没准备好的话应该是出错了
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
            catch (System.Exception e) 
            {
                Debug.LogError(name + " with the range of " + newRange + " tried to scan for targets but failed on: \n" + e);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
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
