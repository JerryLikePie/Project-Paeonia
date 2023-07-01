using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.BuffSystem
{
	public static class BuffConstants
	{
		// 用于枚举 Buff 种类
		// 对于数值 Buff 来说，只有 BuffType 相同才会影响被buff的数值，可以实现一个 Buff 只影响一个数值，或同时影响多个数值的效果

		// todo 目前 buff 和 value 是 1:n 关系，未来如果要实现 m:n 可以改成类型列表，一个 BuffedValue可以被多个 Buff 影响
		public enum BuffType
		{
			BUFF_ATK
		}
	}
}