using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    // 教程显示 UI Panel
    public GameObject tutorialPanel;

    // key: 需要显示教程的关卡 ID
    // val: 全屏教程按钮
    public USerializableDictionary<string, TutorialButton> tutorialMapping;


    void Start()
    {
        string stageName = PlayerPrefs.GetString("Stage_You_Should_Load", "");
        if (tutorialMapping.ContainsKey(stageName))
        {
            tutorialMapping[stageName].gameObject.SetActive(true);
        }
        else
		{
            tutorialPanel.SetActive(false);
		}
    }
}
