using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utilities;

public class TankCombatBehavior : IDollsCombatBehaviour
{
    private DollsCombat context;
    private Queue<Hex> toCancelFog;

    void Start()
    {
        toCancelFog = new Queue<Hex>();
    }

    public override void CheckEnemy(DollsCombat context)
    {
        for (int j = 0; j < context.enemy.Count; j++)
        {
            if (context.enemy[j] == null)
            {
                context.enemy.RemoveAt(j);
                j = 0;
            }
        }
        for (int i = 0; i < context.enemy.Count; i++)
        {
            if (context.enemy[i].gameObject != null && Find_Distance(transform.gameObject, context.enemy[i].gameObject) <= 17.32 * (context.dolls.dolls_range + context.rangeBuff))
            {
                Debug.Log(Find_Distance(transform.gameObject, context.enemy[i].gameObject));
                if (context.enemy[i].enemy.enemy_visible == true && context.enemy[i].gameObject.activeSelf)
                {
                    if (context.canFire)
                    {
                        context.setEnemy = context.enemy[i];
                        context.counter = 0;
                        Debug.Log("ÓÐÚÀ£¡");
                        context.Attack();
                        StartCoroutine(context.FireRate());
                    }
                }
            }
        }
    }
}
