using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 用于 Inspector 注入数据的掉落物类型
/// 
/// 目前仅考虑了数量、概率
/// </summary>
[Serializable]
public struct Loot
{
	// 掉落物类型
	public ItemType lootType;
	// 掉落数量
	public float amount;
	// 掉落概率
	[Range(0, 1)]
	public float probability;
}