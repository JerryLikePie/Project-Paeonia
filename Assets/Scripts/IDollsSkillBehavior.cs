using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IDollsSkillBehavior : MonoBehaviour
{
    [HideInInspector] public long timeStart, timeElapsed;//记录技能开始时间和已经过去的时间
    [HideInInspector] public float percentageTime, timeleft;//已经过去的百分比，用于黑色overlay的长度，以及剩余多少时间
    [HideInInspector] public bool inCoolDown = false;//在不在技能冷却时间里面
    [HideInInspector] public bool isInConstantUse = false;//在不在技能持续时间范围内
    [HideInInspector] public Hex[] allTiles;
    public IDollsSkillBehavior secondSkill;

    public float cooldownTime;//技能冷却时间
    public GameObject showTime;//冷却的时候有个文字来倒数时间
    public GameObject cooldown;//冷却的时候有个黑色的overlay
    public int range;//这个技能的范围是多少
    public DollsCombat unit;//这个技能是属于谁的
    public GameObject mapList;//有些作用在地图上的技能需要知道地图
    //因为别的代码不能直接接触SkillBehavior的子类，所以得在这里声明，子类去用

    public virtual void loadMap()
    {
        List<Hex> temp = new List<Hex>();
        for (int i = 0; i < mapList.transform.childCount; i++)
        {
            try
            {
                if (mapList.transform.GetChild(i).GetComponent<Hex>() != null)
                    temp.Add(mapList.transform.GetChild(i).GetComponent<Hex>());
            }
            catch
            {
                continue;
            }
        }
        allTiles = temp.ToArray();
    }


    public virtual void activateSkill(Transform location)
    {
        //技能
    }

    public virtual void activateSkill()
    {
        //没有范围的技能
    }

    public virtual void passiveTrait(DollsCombat unit)
    {
        //被动
    }

    public void CoolDownPanel()
    {
        //冷却时间的技能图标变黑以及倒数显示，所有技能通用，就放在这里
        // 该技能是瞬间完成的
        if (inCoolDown)
        {
            if (!showTime.activeSelf)
            {
                showTime.SetActive(true);
            }
            timeElapsed = System.DateTime.Now.Ticks - timeStart; // 计算已经cd了多久
            percentageTime = timeElapsed / (cooldownTime * 10000000);
            cooldown.transform.localScale = new Vector3(1.05f * (1 - percentageTime), 1.05f, 1f);
            timeleft = cooldownTime - (timeElapsed / 10000000f);
            showTime.GetComponent<Text>().text = timeleft.ToString("F1");
            if (percentageTime >= 1)
            {
                inCoolDown = false;
                showTime.SetActive(false);
            }
        }
        else if (isInConstantUse)
        {
            if (!showTime.activeSelf)
            {
                showTime.SetActive(true);
            }
            cooldown.transform.localScale = new Vector3(1.05f, 1.05f, 1f);
            showTime.GetComponent<Text>().text = "使用中";
        }
    }
}
