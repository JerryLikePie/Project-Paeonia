using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utilities;

public class DollsPoolManager : MonoBehaviour
{
    public int[] dollsInputted;
    [SerializeField] private AudioSource touchSound;
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
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            touchSound.Play();
        }
    }
    private void OnMouseDown()
    {
        touchSound.Play();
    }

    [SerializeField] public Charactor[] dolls;
    public void InputDolls(int slotNum, int dollsID)
    {
        dollsInputted[slotNum] = dollsID;
    }
}
