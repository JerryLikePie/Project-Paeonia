using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollsUnlockManage : MonoBehaviour
{
    public GameObject[] dolls;
    string currentStage;
    Queue<int> unlockWho;

    private void Start()
    {
        unlockWho = new Queue<int>();
        currentStage = PlayerPrefs.GetString("Stage_You_Should_Load", "0");
        lockCertainDolls();
    }

    void lockCertainDolls()
    {
        switch (currentStage)
        {
            case "Map_T1-1":
                unlockWho.Enqueue(1);
                break;
            case "Map_T1-2":
                unlockWho.Enqueue(1);
                break;
            default:
                unlockWho.Enqueue(1);
                unlockWho.Enqueue(10);
                unlockWho.Enqueue(17);
                break;
        }
        unlockWho.Enqueue(0);
        while (unlockWho.Peek() != 0)
        {
            dolls[unlockWho.Dequeue()].GetComponent<DollsProperty>().dolls_unlocked = true;
        }
    }
}
