using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���ڴ�����˵������ Mangaer
public class LootManager : MonoBehaviour
{
    public GameCore gameCore;

    private bool isRecording = false;

    // �ܼƵ����˶�����Ʒ 
    // ���� -> ����
    private Dictionary<ItemObject, float> totalLoots;

	private void Start()
	{
        totalLoots = new Dictionary<ItemObject, float>();
        // �����з���λ����ɱ�¼�
        gameCore.eventSystem.RegistListener(GameEventType.Event_Enemy_Killed, GetLoot);
        // �������ﱻ���ɵ��¼�
        gameCore.eventSystem.RegistListener(GameEventType.Event_Mine_Gathered, GetLoot);
    }

	// һ����Ϸ/���߿�ͧ�ص�ս��ʱ��ʼʱ���ã���ʼ��¼�����������ݣ�
	// ��ʼ������д������
	public void startRecordLooting()
	{
        totalLoots.Clear();
        isRecording = true;
	}

    // �з���λ����ɱʱ����
    public void GetLoot(GameEventData e)
	{
        Lootable lootableInstance = e.source.GetComponent<Lootable>();
        if (lootableInstance != null) {
            CollectLoot(lootableInstance);
		}
	}

	// �ж���ͳ�Ƶ����������
	public void CollectLoot(Lootable lootableInstance)
	{
        if (!isRecording)
		{
            Debug.LogError("������û�м�¼���������µ��� collectLoot(). ���ȵ��� startRecordLooting() ���������¼");
            return;
		}

        foreach (Loot loot in lootableInstance.loots)
        {
            // ���ݸ����ж��Ƿ������Ʒ
            // ������������
            if (loot.variable)
            {
                float totalAmount = 0;
                if (totalLoots.TryGetValue(loot.lootItem, out totalAmount))
                {
                    totalAmount += loot.amount * Random.Range(0, 1);
                    totalLoots[loot.lootItem] = totalAmount;
                }
                else
                {
                    totalLoots.Add(loot.lootItem, loot.amount);
                }
            }
            else if (loot.probability >= Random.Range(0, 1))
			{
                float totalAmount = 0;
                if (totalLoots.TryGetValue(loot.lootItem, out totalAmount))
				{
                    totalAmount += loot.amount;
                    totalLoots[loot.lootItem] = totalAmount;
				}
                else
				{
                    totalLoots.Add(loot.lootItem, loot.amount);
				}
			}
		}
	} 

    // ��ѯ��ǰ��������
    public Dictionary<ItemObject, float> QueryLoots()
	{
        return new Dictionary<ItemObject, float>(totalLoots);
	}

    // ��Ϸ����ʱֹͣ��¼�����߿�ͧ����ʱ����һ��
    public void StopRecordLooting()
    {
        // ���totalloots�����Ѿ��е���������Щ������������Ϸ������
        foreach (KeyValuePair<ItemObject, float> thisItem in totalLoots)
        {
            gameCore.inventoryManager.AddResource(thisItem.Key, (int)thisItem.Value);
        }
        isRecording = false;
	}
}
