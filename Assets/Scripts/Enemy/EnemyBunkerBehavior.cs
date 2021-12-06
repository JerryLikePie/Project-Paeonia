using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utilities;

public class EnemyBunkerBehavior : IEnemyCombatBehavior
{
    bool canFire;
    void Start()
    {
        canFire = false;
    }
    public override void checkDolls(EnemyCombat context)
    {
       // do nothing
    }
}
