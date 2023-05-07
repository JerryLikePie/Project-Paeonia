using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IEnemyBehavior : MonoBehaviour
{
    [HideInInspector] public bool firstTime = true;
    public virtual void CheckDolls(EnemyCombat context)
    {
        // no-op
    }
    public virtual void FogEvent(EnemyCombat context)
    {
        // nothing
    }
}
