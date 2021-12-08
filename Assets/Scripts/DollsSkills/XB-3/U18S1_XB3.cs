using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class U18S1_XB3 : IDollsSkillBehavior
{
    //ÑÐºäÈý ºäÕ¨
    public override void activateSkill(Transform location)
    {
        inCoolDown = true;
        timeStart = System.DateTime.Now.Ticks;
        unit.supportTargetCord = location;
        ((AttackerCombatBehavior)unit.combatBehaviour).flyEndCord = 2.5f * location.position - unit.transform.position;
        unit.combatBehaviour.CheckEnemy(unit);
        ((AttackerCombatBehavior)unit.combatBehaviour).canAttack = true;
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
