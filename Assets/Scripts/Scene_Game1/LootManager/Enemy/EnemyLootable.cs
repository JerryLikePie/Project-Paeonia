using System.Collections;
using UnityEngine;

// enemy 掉落，当接收到死亡事件时触发
public class EnemyLootable : Lootable
{
	const GameEventSystem.EventType Event_Enemy_Killed = GameEventSystem.EventType.Event_Enemy_Killed;	// 这也太长了(

	protected override void Start()
	{
		base.Start();
		// 注册事件监听函数
		gameCore.eventSystem.registListener(Event_Enemy_Killed, OnEnemyDestroy);
	}

	// 监听 EventSystem 传来的单位被摧毁事件
	// see EnemyCombat.WithDrawl()
	private void OnEnemyDestroy(GameEventSystem.Event e)
	{
		if (e.source == this.gameObject)
		{
			lootManager.collectLoot(this);
			// 触发一次，然后删除
			gameCore.eventSystem.removeListener(Event_Enemy_Killed, OnEnemyDestroy);

			// debug
			Debug.Log("Loots:");
			IDictionary map = lootManager.queryLoots();
			foreach (var k in map.Keys)
			{
				Debug.Log(k + " : " + map[k]);
			}
		}
	}
}