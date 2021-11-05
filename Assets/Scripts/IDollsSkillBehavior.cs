using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IDollsSkillBehavior : MonoBehaviour
{
    public virtual void activateSkill(DollsCombat context, Hex location)
    {
        //技能
        //context - 谁用技能
        //location - 用在哪里
    }

    public virtual void passiveTrait(DollsCombat context)
    {
        //被动
    }
}
