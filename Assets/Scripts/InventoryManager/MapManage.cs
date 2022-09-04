using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManage : MonoBehaviour
{
    public InventoryManager inventory;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        for (int i = 0; i < Object.FindObjectsOfType<MapManage>().Length; i++)
        {
            if (Object.FindObjectsOfType<MapManage>()[i] != this)
            {
                Destroy(Object.FindObjectsOfType<MapManage>()[i].gameObject);
            }
        }
    }

    private void Start()
    {
        inventory = GameObject.FindWithTag("Inventory").GetComponent<InventoryManager>();
    }

    public void EnemyKilled(int num)
    {
        inventory.AddResource(1, num);
    }
    public void UnitSpawned(int num)
    {
        inventory.DecreaseResource(2, num);
    }

}
