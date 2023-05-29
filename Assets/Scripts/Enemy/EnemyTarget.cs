using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utilities;

public class EnemyTarget : IEnemyBehavior
{
    // 类似于靶标的不攻击单位
    public override void CheckDolls(EnemyCombat context)
    {
        // does nothing
    }
}
