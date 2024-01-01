using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 挂载在需要处理掉落的游戏对象上
// 建议参考 EnemyLootable
public class Lootable : MonoBehaviour
{
	[SerializeField] public List<Loot> loots;

	protected GameCore gameCore;
	protected LootManager lootManager;

	// Use this for initialization
	protected virtual void Start()
	{
		gameCore = GameObject.Find("GameCore").GetComponent<GameCore>();
		lootManager = gameCore.lootManager;
	}

	// Update is called once per frame
	void Update()
	{

	}
}