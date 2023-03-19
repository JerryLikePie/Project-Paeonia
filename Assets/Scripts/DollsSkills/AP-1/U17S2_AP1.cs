using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class U17S2_AP1 : IDollsSkillBehavior
{
    //复兴号 侦查
    public override void activateSkill(Transform location)
    {
        Debug.Log(location + " 复兴甲使用2技能");
        ((AttackerCombatBehavior)unit.combatBehaviour).newTask();
        isInConstantUse = true;
        unit.supportTargetCord = location;
        ((AttackerCombatBehavior)unit.combatBehaviour).canAttack = false;
        ((AttackerCombatBehavior)unit.combatBehaviour).useGun = false;
        unit.combatBehaviour.CheckEnemy(unit);
    }
    void delayedCooldown()
    {
        if (isInConstantUse)
        {
            if (((AttackerCombatBehavior)unit.combatBehaviour).taskDone())
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
