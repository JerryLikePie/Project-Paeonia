using UnityEngine;
using System.Collections;

public class IDollsCombatBehaviour : MonoBehaviour
{
    [HideInInspector] public bool firstTime = true;
    public virtual void CheckEnemy(DollsCombat context)
    {
        // no-op
    }
    public virtual void FogEvent(DollsCombat context)
    {
        // lalala
    }
    
}
