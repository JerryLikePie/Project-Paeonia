using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManage : MonoBehaviour
{
    public GameObject tutorialPanel;
    public TutorialButton[] tutorial;
    string stageName;
    // Start is called before the first frame update
    void Start()
    {
        stageName = PlayerPrefs.GetString("Stage_You_Should_Load","0");
        switch (stageName)
        {
            case "TR-1":
                tutorial[0].gameObject.SetActive(true);
                break;
            case "TR-2":
                tutorial[1].gameObject.SetActive(true);
                break;
            case "TR-3":
                tutorial[2].gameObject.SetActive(true);
                break;
            default:
                tutorialPanel.SetActive(false);
                break;
        }
    }
}
