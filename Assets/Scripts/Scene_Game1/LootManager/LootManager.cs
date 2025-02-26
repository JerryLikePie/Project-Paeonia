using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 用于处理敌人掉落物的 Mangaer
public class LootManager : MonoBehaviour
{
    public GameCore gameCore;

    private bool isRecording = false;

    // 总计掉落了多少物品 
    // 类型 -> 数量
    private Dictionary<ItemType, float> totalLoots;

	private void Start()
	{
        totalLoots = new Dictionary<ItemType, float>();
        // 监听敌方单位被击杀事件
        gameCore.eventSystem.RegistListener(GameEventType.Event_Enemy_Killed, OnEnemyKilled);
    }

	// 一局游戏开始时调用，开始记录掉落物（清空数据）
	// 初始化代码写在这里
	public void startRecordLooting()
	{
        totalLoots.Clear();
        isRecording = true;
	}

    // 敌方单位被击杀时触发
    public void OnEnemyKilled(GameEventData e)
	{
        Lootable lootableInstance = e.source.GetComponent<Lootable>();
        if (lootableInstance != null) {
            CollectLoot(lootableInstance);
		}
	}

	// 判定并统计掉落物的数量
	public void CollectLoot(Lootable lootableInstance)
	{
        if (!isRecording)
		{
            Debug.LogError("尝试在没有记录掉落的情况下调用 collectLoot(). 请先调用 startRecordLooting() 开启掉落记录");
            return;
		}

        foreach (Loot loot in lootableInstance.loots)
        {
            // 根据概率判定是否掉落物品
            // 并加入总数中
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

    // 查询当前掉落数量
    public Dictionary<ItemType, float> QueryLoots()
	{
        return new Dictionary<ItemType, float>(totalLoots);
	}

    // 游戏结束时停止记录
    public void StopRecordLooting()
    {
        isRecording = false;
	}
}
