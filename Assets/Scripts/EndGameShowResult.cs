using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGameShowResult : MonoBehaviour
{
    public GameObject Condition1, Condition2, Condition3, Condition4, Time;
    public GameObject Star1,Star2,Star3,Star4,Star4Special;
    public GameObject EndScore,EndWords;
    int howManyStars = 0;
    public ScoreManager scores;
    public InventoryManager inventory;
    void Start()
    {
        scores = Object.FindObjectOfType<ScoreManager>().GetComponent<ScoreManager>();
        //inventory = Object.FindObjectOfType<InventoryManager>().GetComponent<InventoryManager>();
        Conditions();
        Final();
    }
    void Conditions()
    {
        if (scores.captureObjective && !scores.Lost())
        {
            Condition1.GetComponent<Text>().text = "��ռ�������ؼ��ڵ�";
            Star1.SetActive(true);
            howManyStars += 1;
        }
        else
        {
            Condition1.GetComponent<Text>().text = "δ��ռ�������";
            Star1.SetActive(false);
        }
        if (scores.allDestroyed && !scores.Lost())
        {
            Condition2.GetComponent<Text>().text = "����ɸ�����ȫ��Ŀ��";
            Star2.SetActive(true);
            howManyStars += 1;
        }
        else
        {
            Condition2.GetComponent<Text>().text = "δ�����ȫ��Ŀ��";
            Star2.SetActive(false);
        }
        if (scores.noDeath && !scores.Lost())
        {
            Condition3.GetComponent<Text>().text = "DOLLSû���ܵ��ش���ʧ";
            Star3.SetActive(true);
            howManyStars += 1;
        }
        else
        {
            Condition3.GetComponent<Text>().text = "DOLLS�������ش���ʧ";
            Star3.SetActive(false);
        }
        if (scores.inTime && !scores.Lost())
        {
            Condition4.GetComponent<Text>().text = "��սʱ��" + scores.GetTime().ToString("F1")+"���涨ʱ��Ϊ" + scores.GetTimeLimit().ToString("F0");
            if (howManyStars < 3)
            {
                Star4.SetActive(true);
                Star4Special.SetActive(false);
            }
            else
            {
                Star4.SetActive(false);
                Star4Special.SetActive(true);
            }
            howManyStars += 1;
        }
        else 
        {
            Condition4.GetComponent<Text>().text = "��սʱ��" + scores.GetTime().ToString("F1") + "���涨ʱ��Ϊ" + scores.GetTimeLimit().ToString("F0");
            Star4.SetActive(false);
            Star4Special.SetActive(false);
        }
    }
    void Final()
    {
        Debug.Log(scores.dropAmmount);
        //inventory.menuInventory.AddItem(inventory.itemPool[scores.dropID].item, scores.dropAmmount);
        int previousRun = PlayerPrefs.GetInt(scores.stageName, 0);
        if (howManyStars > previousRun)
        {
            PlayerPrefs.SetInt(scores.stageName, howManyStars);
            PlayerPrefs.Save();
        }
        if (scores.Lost())
        {
            EndScore.GetComponent<Text>().text = "��սʧ��";
            //EndWords.GetComponent<Text>().text = "�������ڱƽ�ǰ��ָ���������������ˡ�";
        }
        else if (howManyStars == 1)
        {
            EndScore.GetComponent<Text>().text = "��ս�ɹ�";
            //EndWords.GetComponent<Text>().text = "��Ȼռ���˸õ���������������Ϣ��";
        }
        else if (howManyStars == 2)
        {
            EndScore.GetComponent<Text>().text = "��ս�ɹ�";
            //EndWords.GetComponent<Text>().text = "�����������սĿ�꣬�����ˡ�";
        }
        else if (howManyStars == 3)
        {
            EndScore.GetComponent<Text>().text = "��ս�ɹ�";
            //EndWords.GetComponent<Text>().text = "�����������սĿ�꣬��ϲ�ɺء�";
        }
        else if (howManyStars > 3)
        {
            EndScore.GetComponent<Text>().text = "��ս��ɹ�";
            //EndWords.GetComponent<Text>().text = "�����и������˳�����Ŷ����ʤ�̣��㲻��������ɣ�";
        }
    }
}
