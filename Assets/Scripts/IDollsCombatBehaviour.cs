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
    
    public virtual void WreckAfterDead(DollsCombat context, Transform location)
    {
        // nothing
        // 统一的死后生成残骸行为，每个特定behavior里面override就可以了
    }
}
