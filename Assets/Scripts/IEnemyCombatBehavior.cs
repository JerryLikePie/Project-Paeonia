using UnityEngine;
using System.Collections;

public class IEnemyCombatBehavior : MonoBehaviour
{
    [HideInInspector] public bool firstTime = true;
    public virtual void checkDolls(EnemyCombat context)
    {
        // no-op
    }
    public virtual void lightBulbEvent(EnemyCombat context)
    {
        // lalala
    }
    public virtual void attack(EnemyCombat context)
    {
        // no-op
    }

}
