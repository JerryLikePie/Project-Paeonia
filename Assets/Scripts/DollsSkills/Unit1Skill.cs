using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit1Skill : MonoBehaviour
{
    public DollsCombat unit;
    public int unitID;
    public int skillID;
    public GameObject mapList;
    public Hex toCancelFog;
    public GameObject flare;
    public bool inCoolDown = false;
    public bool canSpawn = false;
    public GameObject cooldown;//冷却的时候有个黑色的overlay
    public float cooldownTime,percentageTime;
    public float skillRange;
    float timeleft;
    public GameObject showTime;
    long timeStart, timeElapsed;
    GameObject newFlare;
    public Transform whereToSkill;

    // Start is called before the first frame update
    void Start()
    {
        cooldown.transform.localScale = new Vector3(0, 1.05f, 1f);
        showTime.SetActive(false);
    }
    public void Can_Spawn()
    {
        canSpawn = true;
    }
    public void CannotSpawn()
    {
        canSpawn = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (unit != null && unit.health <= 0)
        {
            cooldown.transform.localScale = new Vector3(1.05f, 1.05f, 1f);
            inCoolDown = true;
        }
        else
        {
            if (canSpawn == true && inCoolDown == false 
                && Vector3.Distance(whereToSkill.position,unit.transform.position) < 17.5 * skillRange)
            {
                showTime.SetActive(true);
                Invoke("Skill_"+ unitID + "_Activate" + skillID, 0f);
                inCoolDown = true;
            }
            CoolDownPanel();
        }
        try
        {
            if (transform.GetComponent<DragSpawnManager>().isDragging && inCoolDown == false)
            {
                unit.GetComponent<Unit>().skillBox.SetActive(true);
            }
            else
            {
                unit.GetComponent<Unit>().skillBox.SetActive(false);
            }
        }
        catch { }
    }
    public void CoolDownPanel()
    {
        if (inCoolDown)
        {
            showTime.SetActive(true);
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
    }
    public void Skill_1_Activate1()
    {
        Debug.Log(whereToSkill);
        inCoolDown = true;
        timeStart = System.DateTime.Now.Ticks;
        //Unit.killSound.Play();
        StartCoroutine(LightFor30Sec());
    }
    IEnumerator LightFor30Sec()
    {
        yield return new WaitForSeconds(1);
        newFlare = Instantiate(flare, whereToSkill.position, Quaternion.identity);
        for (int i = 0; i <= mapList.transform.childCount - 1; i++)
        {
            try
            {
                toCancelFog = mapList.transform.GetChild(i).GetComponent<Hex>();
                if (Vector3.Distance(newFlare.transform.position, toCancelFog.transform.position) <= 17.32 * 3)
                {
                    toCancelFog.isInFog += 1;
                    toCancelFog.ChangeTheFog();
                }
            }
            catch
            {
                continue;
            }
        }
        yield return new WaitForSeconds(17);
        for (int i = 0; i <= mapList.transform.childCount - 1; i++)
        {
            try
            {
                toCancelFog = mapList.transform.GetChild(i).GetComponent<Hex>();
                if (Vector3.Distance(newFlare.transform.position, toCancelFog.transform.position) <= 17.32 * 3)
                {
                    toCancelFog.isInFog -= 1;
                    toCancelFog.ChangeTheFog();
                }
            }
            catch
            {
                continue;
            }
        }
        Destroy(newFlare);
    }

    public void Skill_10_Activate1()
    {
        Debug.Log(whereToSkill);
        inCoolDown = true;
        timeStart = System.DateTime.Now.Ticks;
        unit.supportTargetCord = whereToSkill;
        unit.combatBehaviour.CheckEnemy(unit);
    }
    public void Skill_17_Activate1()
    {
        Debug.Log(unit.name);
        inCoolDown = true;
        timeStart = System.DateTime.Now.Ticks;
        unit.supportTargetCord = whereToSkill;
        ((AttackerCombatBehavior)unit.combatBehaviour).flyEndCord = 2.5f * whereToSkill.position - unit.transform.position;
        unit.combatBehaviour.CheckEnemy(unit);
    }

}
