using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utilities;

public class DollsPoolManager : MonoBehaviour
{
    public int[] dollsInputted;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        for (int i = 0; i < Object.FindObjectsOfType<DollsPoolManager>().Length; i++)
        {
            if (Object.FindObjectsOfType<DollsPoolManager>()[i] != this)
            {
                Destroy(Object.FindObjectsOfType<DollsPoolManager>()[i].gameObject);
            }
        }
    }

    [SerializeField] public Charactor[] dolls;
    public void InputDolls(int slotNum, int dollsID)
    {
        dollsInputted[slotNum] = dollsID;
    }
}
