using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManage : MonoBehaviour
{
    public GameObject T1, T2;
    string stageName;
    // Start is called before the first frame update
    void Start()
    {
        stageName = PlayerPrefs.GetString("Stage_You_Should_Load","0");
        if (PlayerPrefs.GetInt(stageName, 0) == 0)
        {
            switch (stageName)
            {
                case "Map_T1-1":
                    T1.SetActive(true);
                    break;
                case "Map_T1-2":
                    T2.SetActive(true);
                    break;
                default:
                    //do nothing
                    break;
            }
        }
    }
}
