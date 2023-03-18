using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IDollsSkillBehavior : MonoBehaviour
{
    [HideInInspector] public long timeStart, timeElapsed;//��¼���ܿ�ʼʱ����Ѿ���ȥ��ʱ��
    [HideInInspector] public float percentageTime, timeleft;//�Ѿ���ȥ�İٷֱȣ����ں�ɫoverlay�ĳ��ȣ��Լ�ʣ�����ʱ��
    [HideInInspector] public bool inCoolDown = false;//�ڲ��ڼ�����ȴʱ������
    [HideInInspector] public bool isInConstantUse = false;//�ڲ��ڼ��ܳ���ʱ�䷶Χ��
    [HideInInspector] public Hex[] allTiles;
    public IDollsSkillBehavior secondSkill;

    public float cooldownTime;//������ȴʱ��
    public GameObject showTime;//��ȴ��ʱ���и�����������ʱ��
    public GameObject cooldown;//��ȴ��ʱ���и���ɫ��overlay
    public int range;//������ܵķ�Χ�Ƕ���
    public DollsCombat unit;//�������������˭��
    public GameObject mapList;//��Щ�����ڵ�ͼ�ϵļ�����Ҫ֪����ͼ
    //��Ϊ��Ĵ��벻��ֱ�ӽӴ�SkillBehavior�����࣬���Ե�����������������ȥ��

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
        //����
    }

    public virtual void activateSkill()
    {
        //û�з�Χ�ļ���
    }

    public virtual void passiveTrait(DollsCombat unit)
    {
        //����
    }

    public void CoolDownPanel()
    {
        //��ȴʱ��ļ���ͼ�����Լ�������ʾ�����м���ͨ�ã��ͷ�������
        // �ü�����˲����ɵ�
        if (inCoolDown)
        {
            if (!showTime.activeSelf)
            {
                showTime.SetActive(true);
            }
            timeElapsed = System.DateTime.Now.Ticks - timeStart; // �����Ѿ�cd�˶��
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
            showTime.GetComponent<Text>().text = "ʹ����";
        }
    }
}
