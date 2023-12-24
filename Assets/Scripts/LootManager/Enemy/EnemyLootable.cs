using System.Collections;
using UnityEngine;

// enemy 掉落，当接收到死亡事件时触发
public class EnemyLootable : Lootable
{

	// 监听 EventSystem 传来的单位被摧毁事件
	// see EnemyCombat.WithDrawl()
	private void OnEnemyDestroy()
	{
		lootManager.collectLoot(this);
		// debug
		Debug.Log("Loots:");
		IDictionary map = lootManager.queryLoots();
		foreach (var k in map.Keys)
		{
			Debug.Log(k + " : " + map[k]);
		}
	}
}