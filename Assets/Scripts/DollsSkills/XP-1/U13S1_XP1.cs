using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class U13S1_XP1 : IDollsSkillBehavior
{
    //ÑÐÇý1£ºÖÆ¿Õ
    public override void activateSkill(Transform location)
    {
        Debug.Log(location);
        inCoolDown = true;
        timeStart = System.DateTime.Now.Ticks;
        unit.supportTargetCord = location;
        unit.combatBehaviour.CheckEnemy(unit);
        ((FighterCombatBehavior)unit.combatBehaviour).canAttack = true;
    }

    void Start()
    {
        cooldown.transform.localScale = new Vector3(0, 1.05f, 1f);
        showTime.SetActive(false);
    }
    void Update()
    {
        if (unit != null && unit.health <= 0)
        {
            cooldown.transform.localScale = new Vector3(1.05f, 1.05f, 1f);
            inCoolDown = true;
        }
        else
        {
            CoolDownPanel();
        }
    }
}
