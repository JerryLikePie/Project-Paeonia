using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���ڴ�����˵������ Mangaer
public class LootManager : MonoBehaviour
{
    private bool isRecording = false;

    // �ܼƵ����˶�����Ʒ 
    // ���� -> ����
    private Dictionary<ItemType, float> totalLoots = new Dictionary<ItemType, float>();

    // һ����Ϸ��ʼʱ���ã���ʼ��¼������
    // ��ʼ������д������
    public void startRecordLooting()
	{;
        totalLoots.Clear();
        isRecording = true;
	}

    // ���ⲿ����
    // �ж���ͳ�Ƶ����������
    public void collectLoot(Lootable lootable)
	{
        if (!isRecording)
		{
            Debug.LogWarning("������û�м�¼���������µ��� collectLoot(). ���ȵ��� startRecordLooting() ���������¼");
            return;
		}

        foreach (Loot loot in lootable.loots)
        {
            // ���ݸ����ж��Ƿ������Ʒ
            // ������������
            if (loot.probability >= Random.Range(0, 1))
			{
                float totalAmount = 0;
                if (totalLoots.TryGetValue(loot.lootType, out totalAmount))
				{
                    totalAmount += loot.amount;
                    totalLoots[loot.lootType] = totalAmount;
				}
                else
				{
                    totalLoots.Add(loot.lootType, loot.amount);
				}
			}
		}
	} 

    // ��ѯ��ǰ��������
    public Dictionary<ItemType, float> queryLoots()
	{
        return new Dictionary<ItemType, float>(totalLoots);
	}

    // ��Ϸ����ʱֹͣ��¼
    // ��յ����б�
    public void stopRecordLooting()
    {
        isRecording = false;
	}
}
