using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class U13S1_XP1 : IDollsSkillBehavior
{
    //研驱1：对地
    public override void activateSkill(Transform location)
    {
        Debug.Log(location + " 研驱1使用1技能");
        ((FighterCombatBehavior)unit.combatBehaviour).newTask();
        isInConstantUse = true;
        unit.supportTargetCord = location;
        ((FighterCombatBehavior)unit.combatBehaviour).canAttack = true;
        unit.combatBehaviour.CheckEnemy(unit);
        //((FighterCombatBehavior)unit.combatBehaviour).canAttack = true;
    }

    void delayedCooldown()
    {
        if (isInConstantUse)
        {
            if (((FighterCombatBehavior)unit.combatBehaviour).taskDone())
            {
                isInConstantUse = false;
                inCoolDown = true;
                timeStart = System.DateTime.Now.Ticks;
            }
        }
        CoolDownPanel();
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
        else if (unit != null)
        {
            delayedCooldown();
        }
    }
}
