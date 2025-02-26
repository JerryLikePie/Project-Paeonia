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
    private Dictionary<ItemType, float> totalLoots;

	private void Start()
	{
        totalLoots = new Dictionary<ItemType, float>();
        // �����з���λ����ɱ�¼�
        gameCore.eventSystem.RegistListener(GameEventType.Event_Enemy_Killed, OnEnemyKilled);
    }

	// һ����Ϸ��ʼʱ���ã���ʼ��¼�����������ݣ�
	// ��ʼ������д������
	public void startRecordLooting()
	{
        totalLoots.Clear();
        isRecording = true;
	}

    // �з���λ����ɱʱ����
    public void OnEnemyKilled(GameEventData e)
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
    public Dictionary<ItemType, float> QueryLoots()
	{
        return new Dictionary<ItemType, float>(totalLoots);
	}

    // ��Ϸ����ʱֹͣ��¼
    public void StopRecordLooting()
    {
        isRecording = false;
	}
}
