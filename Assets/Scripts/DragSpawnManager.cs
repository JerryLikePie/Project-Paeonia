using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragSpawnManager : MonoBehaviour
{
    [SerializeField] DragSpawnItem[] slots;
    [SerializeField] Image[] slotsImage;
    public GameObject enemyList;
    public GameObject playerList;
    public MapCreate map;
    // Start is called before the first frame update
    void Start()
    {
        loadDolls();
    }

    public void loadDolls()
    {
        DollsPoolManager dollManager = GameObject.Find("DollsPool").GetComponent<DollsPoolManager>();
        for (int i = 0; i < 6; i++)
        {
            if (dollManager.dollsInputted[i] != 0)
            {
                slotsImage[i].sprite = dollManager.dolls[i + 1].avatar;
                slots[i].spawn = dollManager.dolls[i + 1].doll;

            }
        }
    }
}
