using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]


public class Inventory : ScriptableObject
{
    string about = "���ϵ�������Ϊ��" +
        "������Ʒ" +
        "��������" +
        "�߼�����";
    public List<InventorySlot> container = new List<InventorySlot>();
    public void AddItem(ItemObject item, int amount)
    {
        bool hasItem = false;
        for (int i = 0; i < container.Count; i++)
        {
            if (container[i].isItem(item))
            {
                container[i].AddAmount(amount);
                hasItem = true;
                break;
            }
        }
        if (!hasItem)
        {
            container.Add(new InventorySlot(item, amount));
        }
    }

    public void MinusItem(ItemObject item, int amount)
    {
        bool hasItem = false;
        for (int i = 0; i < container.Count; i++)
        {
            if (container[i].isItem(item))
            {
                if (container[i].amount < amount)
                {
                    return;
                }
                container[i].AddAmount(-amount);
                hasItem = true;
                break;
            }
        }
        if (!hasItem)
        {
            // û�и����壬�Ͳ���
        }
    }
}


[System.Serializable]
public class InventorySlot
{
    public ItemObject item;
    public int amount;
    public InventorySlot(ItemObject item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }

    public void AddAmount(int num)
    {
        amount += num;
    }

    public bool isItem(ItemObject item) 
    {
        return (item == this.item);
    }
}
