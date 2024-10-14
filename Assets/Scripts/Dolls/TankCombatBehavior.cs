using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utilities;

public class TankCombatBehavior : IDollsCombatBehaviour
{
    private DollsCombat context;
    int newRange;

    void Start()
    {
    }

    public override void WreckAfterDead(DollsCombat context, Transform deadbody)
    {
        if (context.wreckage == null)
        {
            // 没有绑定残骸吗？那就不生成了
            Debug.Log("No wreckage?");
            return;
        }
        // TODO: 写一个陆地dolls的残骸脚本
        GameObject body = Instantiate(context.wreckage, deadbody.position, Quaternion.identity);
        body.SetActive(true);
    }

    public override void CheckEnemy(DollsCombat context)
    {
        for (int j = 0; j < context.enemyList.Count; j++)
        {
            try
            {
                if (context.enemyList[j] == null)
                {
                    context.enemyList.RemoveAt(j);
                    j = 0;
                    continue;
                }
                if (context.enemyList[j].getType() == 5)
                {
                    // 空军不考虑
                    context.enemyList.RemoveAt(j);
                    j = 0;
                    continue;
                }
            }
            catch
            {
                continue;
            }
        }
        if (context.canFire)
        {
            float nearest = 99999f;
            float distance;
            int number = -1;
            for (int i = 0; i < context.enemyList.Count; i++)
            {
                if (context.enemyList[i] != null)
                {
                    if (context.enemyList[i].enemy.enemy_visible == true && context.enemyList[i].gameObject.activeSelf)
                    {
                        if (context.rangeBuff > 0)
                        {
                            newRange = context.dolls.dolls_range;
                        }
                        else
                        {
                            newRange = context.dolls.dolls_range + (int)context.rangeBuff;
                        }
                        distance = FindDistance(transform.gameObject, context.enemyList[i].gameObject);
                        if (distance <= 17.32 * newRange)
                        {
                            if (!context.map.IsBlocked(context.currentTile, context.enemyList[i].transform.position))
                            {
                                if (distance < nearest)
                                {
                                    number = i;
                                    nearest = distance;
                                }
                            }
                        }
                    }
                }
            }
            if (number >= 0)
            {
                context.setEnemy = context.enemyList[number];
                context.counter = 0;
                context.Attack();
                StartCoroutine(context.FireRate());
            }
        }
    }
}
