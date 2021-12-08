using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class U5S1_T3485 : IDollsSkillBehavior
{
    bool haveAPDS = false;
    public GameObject text;
    public AudioSource skillSound;
    public override void activateSkill()
    {
        skillSound.Play();
        if (!inCoolDown)
        {
            if (haveAPDS)
            {
                unit.dolls.SwitchAmmo(3);
                unit.dolls.RemoveAtkBuff();
                unit.dolls.RemovePenBuff();
                unit.dolls.RemoveSpdBuff();
                text.SetActive(false);
            }
            else
            {
                text.SetActive(true);
                unit.dolls.SwitchAmmo(5);
                unit.dolls.Buff(-50, 0.7f, 45, 9999);
            }
            haveAPDS = !haveAPDS;
            unit.combatBehaviour.firstTime = true;
            timeStart = System.DateTime.Now.Ticks;
            inCoolDown = true;
            showTime.SetActive(true);
            
        }
    }

    void Start()
    {
        cooldown.transform.localScale = new Vector3(0, 1.05f, 1f);
        showTime.SetActive(false);
        inCoolDown = true;
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
