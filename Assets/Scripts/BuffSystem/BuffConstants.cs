using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.BuffSystem
{
	public static class BuffConstants
	{
		// 用于枚举 Buff 种类
		// 每个 BuffType 相当于一个频道，BuffedAttr 只会受到相同频道的 Buff 的影响

		// 目前暂时有 VAL(Value), EOT(Effect Over Time) 两大类

		/* todo
		/ 目前 Buff-频道，BuffedAttr-频道 之间都是 n:1 关系
		/ 因此 Buff-频道-BuffedAttr 是 m:1:n 关系
		/
		/ 如果要实现 Buff 和 BuffedAttr 的 m:n 关系
		/ 需要将 Buff-频道、BuffedAttr-频道都改为 n:m 关系
		/ 说人话就是 Buff、BuffedAttr 可以拥有多个不同的 BuffId
		*/
		public enum BuffType
		{
			BUFF_ATTR,		// 附加在属性上的 buff, 只结算一次
			BUFF_VAL_EOT	// 附加在数值上的 buff, 会持续修改数值
		}

		public enum BuffId
		{
			BUFF_ATTR_ATK,
			BUFF_VAL_EOT_BLEED
		}
	}
}