using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 挂载在需要处理掉落的游戏对象上
// 建议参考 EnemyLootable
public class Lootable : MonoBehaviour
{
	//将这个脚本挂在击杀后会掉落物资的单位上面
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