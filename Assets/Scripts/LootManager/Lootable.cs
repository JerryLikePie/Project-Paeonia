using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 挂载在需要处理掉落的游戏对象上
// 建议参考 EnemyLootable
public class Lootable : MonoBehaviour
{
	[SerializeField] public List<Loot> loots;

	protected LootManager lootManager;

	// Use this for initialization
	void Start()
	{
		lootManager = GameObject.Find("LootManager").GetComponent<LootManager>();
	}

	// Update is called once per frame
	void Update()
	{

	}
}