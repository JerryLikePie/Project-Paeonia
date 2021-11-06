using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class U10S1_M10 : IDollsSkillBehavior
{
    [SerializeField]
    AudioSource skillSound;

    public override void activateSkill(Transform location)
    {
        inCoolDown = true;
        timeStart = System.DateTime.Now.Ticks;
        unit.supportTargetCord = location;
        unit.combatBehaviour.CheckEnemy(unit);
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
    void playReloaded()
    {
        if (timeleft <= 15000000f && !skillSound.isPlaying && timeleft > 10000000f)
        {
            skillSound.Play();
        }
    }
}
