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
    public ScoreManager Scores;
    void Start()
    {
        Scores = Object.FindObjectOfType<ScoreManager>().GetComponent<ScoreManager>();
        Conditions();
        Final();
    }
    void Conditions()
    {
        if (Scores.captureObjective&& !Scores.friendlyBaseCaptured)
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
        if (Scores.allDestroyed && !Scores.friendlyBaseCaptured)
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
        if (Scores.noDeath && !Scores.friendlyBaseCaptured)
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
        if (Scores.inTime && !Scores.friendlyBaseCaptured)
        {
            Condition4.GetComponent<Text>().text = "��սʱ��" + Scores.timeTook.ToString("F1")+"���涨ʱ��Ϊ" + Scores.timeLimit.ToString("F0");
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
            Condition4.GetComponent<Text>().text = "��սʱ��" + Scores.timeTook.ToString("F1") + "���涨ʱ��Ϊ" + Scores.timeLimit.ToString("F0");
            Star4.SetActive(false);
            Star4Special.SetActive(false);
        }
    }
    void Final()
    {
        int previousRun = PlayerPrefs.GetInt(Scores.stageName, 0);
        if (howManyStars > previousRun)
        {
            PlayerPrefs.SetInt(Scores.stageName, howManyStars);
            PlayerPrefs.Save();
        }
        if (Scores.friendlyBaseCaptured)
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
