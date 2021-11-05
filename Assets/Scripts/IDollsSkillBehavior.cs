using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IDollsSkillBehavior : MonoBehaviour
{
    public virtual void activateSkill(DollsCombat context, Hex location)
    {
        //����
        //context - ˭�ü���
        //location - ��������
    }

    public virtual void passiveTrait(DollsCombat context)
    {
        //����
    }
}
