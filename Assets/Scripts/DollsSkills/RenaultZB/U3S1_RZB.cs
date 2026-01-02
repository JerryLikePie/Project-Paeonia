using System;
using System.Collections.Generic;
using Assets.Scripts.BuffSystem;
using Assets.Scripts.BuffSystem.BuffImpl;
using UnityEngine;

public class U3S1_RZB : IDollsSkillBehavior
{
    [SerializeField] AudioSource skillSound;
    [SerializeField] float toggleCooldown = 4f;

    bool isOn = false;

    readonly List<Buff> selfArmorBuffs = new List<Buff>();
    readonly Dictionary<DollsCombat, List<Buff>> allyBuffs = new Dictionary<DollsCombat, List<Buff>>();

    public override void activateSkill()
    {
        if (inCoolDown)
        {
            return;
        }

        if (skillSound != null)
        {
            skillSound.Play();
        }

        if (!isOn)
        {
            TurnOn();
        }
        else
        {
            TurnOff();
        }

        timeStart = DateTime.Now.Ticks;
        cooldownTime = toggleCooldown;
        inCoolDown = true;
        showTime.SetActive(true);
    }

    void TurnOn()
    {
        ApplySelfArmorDebuff();
        ApplyBuffToAllies();
        isOn = true;
        isInConstantUse = true;
    }

    void TurnOff()
    {
        RemoveSelfArmorDebuff();
        RestoreAllies();
        isOn = false;
        isInConstantUse = false;
    }

    void ApplySelfArmorDebuff()
    {
        if (unit == null)
        {
            return;
        }
        var bm = unit.GetComponent<BuffManager>();
        if (bm == null)
        {
            return;
        }

        if (selfArmorBuffs.Count == 0)
        {
            selfArmorBuffs.Add(new BuffArmorZeroFloat(BuffConstants.BuffId.BUFF_ATTR_ARMOR_FRONT));
            selfArmorBuffs.Add(new BuffArmorZeroInt(BuffConstants.BuffId.BUFF_ATTR_ARMOR_SIDE));
            selfArmorBuffs.Add(new BuffArmorZeroInt(BuffConstants.BuffId.BUFF_ATTR_ARMOR_BACK));
        }

        foreach (var b in selfArmorBuffs)
        {
            bm.removeBuff(b); // ∑¿÷ÿ∏¥
            bm.addBuff(b);
        }
    }

    void RemoveSelfArmorDebuff()
    {
        if (unit == null)
        {
            return;
        }
        var bm = unit.GetComponent<BuffManager>();
        if (bm == null)
        {
            return;
        }
        foreach (var b in selfArmorBuffs)
        {
            bm.removeBuff(b);
        }
    }

    void ApplyBuffToAllies()
    {
        allyBuffs.Clear();

        if (unit == null || unit.allDolls == null)
        {
            return;
        }

        for (int i = 0; i < unit.allDolls.transform.childCount; i++)
        {
            DollsCombat ally = unit.allDolls.transform.GetChild(i).GetComponent<DollsCombat>();
            if (ally == null || ally == unit)
            {
                continue;
            }

            var bm = ally.GetComponent<BuffManager>();
            if (bm == null)
            {
                continue;
            }

            var buffsForThis = new List<Buff>
            {
                new BuffDamageMultiplier(1.3f),
                new BuffAccuracyMultiplier(1.4f)
            };

            foreach (var b in buffsForThis)
            {
                bm.removeBuff(b); // ∑¿÷ÿ∏¥
                bm.addBuff(b);
            }

            allyBuffs[ally] = buffsForThis;
        }
    }

    void RestoreAllies()
    {
        foreach (var kvp in allyBuffs)
        {
            if (kvp.Key == null)
            {
                continue;
            }
            var bm = kvp.Key.GetComponent<BuffManager>();
            if (bm == null)
            {
                continue;
            }
            foreach (var b in kvp.Value)
            {
                bm.removeBuff(b);
            }
        }
        allyBuffs.Clear();
    }

    void Start()
    {
        cooldown.transform.localScale = new Vector3(0, 1.05f, 1f);
        showTime.SetActive(false);
        inCoolDown = false;
        isInConstantUse = false;
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

    void OnDisable()
    {
        if (isOn)
        {
            TurnOff();
        }
    }
}

