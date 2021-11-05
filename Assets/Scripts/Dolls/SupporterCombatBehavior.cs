using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupporterCombatBehavior : IDollsCombatBehaviour
{
    private DollsCombat context;
    private Queue<Hex> toCancelFog;

    void Start()
    {
        toCancelFog = new Queue<Hex>();
    }

    public override void CheckEnemy(DollsCombat context)
    {
        if (context.supportTargetCord != null && context.canFire)
        {
            context.Attack();
            Invoke("ResetCord", context.resetTime);
            StartCoroutine(context.FireRate());
            context.counter = 0;
        }
    }
}
