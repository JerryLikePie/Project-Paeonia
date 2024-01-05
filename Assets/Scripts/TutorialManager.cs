using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    // �̳���ʾ UI Panel
    public GameObject tutorialPanel;

    // key: ��Ҫ��ʾ�̵̳Ĺؿ� ID
    // val: ȫ���̳̰�ť
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
