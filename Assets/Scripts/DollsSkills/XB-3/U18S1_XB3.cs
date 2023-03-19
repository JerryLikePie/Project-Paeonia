using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class U18S1_XB3 : IDollsSkillBehavior
{
    //研轰三 轰炸
    public override void activateSkill(Transform location)
    {
        Debug.Log(location + "XB-3释放1技能");
        ((BomberCombatBehavior)unit.combatBehaviour).newTask();
        isInConstantUse = true;
        unit.supportTargetCord = location;
        ((BomberCombatBehavior)unit.combatBehaviour).canAttack = true;
        unit.combatBehaviour.CheckEnemy(unit);
    }
    void delayedCooldown()
    {
        if (isInConstantUse)
        {
            if (((BomberCombatBehavior)unit.combatBehaviour).taskDone())
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
