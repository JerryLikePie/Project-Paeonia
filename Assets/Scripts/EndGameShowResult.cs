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
    
    public InventoryManager inventory;

    public SceneMessager sceneManager;

    void Start()
    {
        //inventory = Object.FindObjectOfType<InventoryManager>().GetComponent<InventoryManager>();

        sceneManager = GameObject.Find("SceneMessager").GetComponent<SceneMessager>();

        // 获取跨场景得分数据
        ScoreManager.GameScoreInfo scores = sceneManager.LoadData<ScoreManager.GameScoreInfo>("game1.scores");
        // 解析和显示
        CheckConditions(scores);
        Final(scores);
    }

    void CheckConditions(ScoreManager.GameScoreInfo scores)
    {
        // 结算时查看是否达成目标
        // 如果是教程关则无视
        // TODO 改一改整个判定系统

        if (scores.captureObjective && !scores.lost)
        {
            Condition1.GetComponent<Text>().text = "已占领该区域关键节点";
            Star1.SetActive(true);
            howManyStars += 1;
        }
        else
        {
            Condition1.GetComponent<Text>().text = " ";
            Star1.SetActive(false);
        }
        if (scores.allDestroyed && !scores.lost)
        {
            Condition2.GetComponent<Text>().text = "已清缴该区域全部目标";
            Star2.SetActive(true);
            howManyStars += 1;
        }
        else
        {
            Condition2.GetComponent<Text>().text = " ";
            Star2.SetActive(false);
        }
        if (scores.noDeath && !scores.lost)
        {
            Condition3.GetComponent<Text>().text = "DOLLS没有受到重大损失";
            Star3.SetActive(true);
            howManyStars += 1;
        }
        else
        {
            Condition3.GetComponent<Text>().text = " ";
            Star3.SetActive(false);
        }
        if (scores.inTime && !scores.lost)
        {
            Condition4.GetComponent<Text>().text = "作战时间" + scores.timeTook.ToString("F1")+"，规定时间为" + scores.timeLimit.ToString("F0");
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
            Condition4.GetComponent<Text>().text = "作战时间" + scores.timeTook.ToString("F1") + "，规定时间为" + scores.timeLimit.ToString("F0");
            Star4.SetActive(false);
            Star4Special.SetActive(false);
        }
    }

    void Final(ScoreManager.GameScoreInfo scores)
    {
        //inventory.menuInventory.AddItem(inventory.itemPool[scores.dropID].item, scores.dropAmmount);
        int previousRun = PlayerPrefs.GetInt(scores.stageName, 0);
        if (howManyStars > previousRun)
        {
            PlayerPrefs.SetInt(scores.stageName, howManyStars);
            PlayerPrefs.Save();
        }
        if (scores.lost)
        {
            EndScore.GetComponent<Text>().text = "作战失败";
            //EndWords.GetComponent<Text>().text = "灾兽正在逼近前哨指挥所，请立即撤退。";
        }
        else if (howManyStars == 1)
        {
            EndScore.GetComponent<Text>().text = "作战成功";
            //EndWords.GetComponent<Text>().text = "虽然占领了该地区，但还不能休息。";
        }
        else if (howManyStars == 2)
        {
            EndScore.GetComponent<Text>().text = "作战成功";
            //EndWords.GetComponent<Text>().text = "基本完成了作战目标，辛苦了。";
        }
        else if (howManyStars == 3)
        {
            EndScore.GetComponent<Text>().text = "作战成功";
            //EndWords.GetComponent<Text>().text = "完美完成了作战目标，可喜可贺。";
        }
        else if (howManyStars > 3)
        {
            EndScore.GetComponent<Text>().text = "作战大成功";
            //EndWords.GetComponent<Text>().text = "听闻有个代理人成天带着队伍打胜仗，你不会就是他吧？";
        }
    }
}
