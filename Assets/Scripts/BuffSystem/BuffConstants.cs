using UnityEngine;

namespace Assets.Scripts.BuffSystem
{
	public static class BuffConstants
	{
		/* ┳┣┃┗ */

		/// 用于枚举 Buff 种类
		/// buff 系统会根据 buffType 的不同做区别处理

		/// 目前的种类目录：
		/// ┳ ATTR(Attribute, 属性)
		/// ┗ VAL(Value, 普通数值)
		///    ┗ VAL_EOT(Effect Of Time)

		/// ATTR 类：
		///    ATTR 用于实现属性值上的 buff 效果
		///    Buffee 为 <see cref="BuffedAttr{T}"/> <see cref="BuffedAttrAttribute"/> 等
		///    Buff 基类为 <see cref="AttrBuff{T}"/>
		///    
		///    每个 buffed-attr 只有一个 buffId，但可以有多个同 buffId 的 buff 作用在该 buffed-attr 上
		///    实现逻辑为：
		///        1. buffed-attr 作为 listener 注册到本地的 buffManager 上
		///        2. buffManager 在 buff 变更后通知 buffed-attr
		///        3. buffed-attr 收到通知后，调用 buffManager 的 takeAllEffects 来结算所有同 buffId 的 buff 效果
		///	   上述逻辑可以保证 buffed-attr 始终为最新

		/// VAL 类：
		///    VAL 用于实现一般数值上的 buff 效果
		///    每个 buffed-val 可以有多个 buffId
		///    实现逻辑为：
		///        1. buffed-val 作为 listener 注册到本地的 buffManager 上
		///        2. buffManager 在 buff 触发时通知 buffed-val
		///        3. buffed-val 收到通知后，调用该 buff 的 takeEffect ，基于当前值结算一次 buff 效果
		///    上述逻辑让 buffed-val 的值可以安全地被多个不同类型的 buff 修改(例如单次伤害、单次回复、流血、再生都是作用于生命值属性上的)

		/// VAL_EOT 类：
		///    VAL_EOT 用于实现一般数值上的持续型 buff 效果，从处理逻辑上来说派生自 VAL 类
		///    Buffee 为 <see cref="BuffedValue{T}"/> 等
		///    Buff 基类为 <see cref="EffectOverTimeBuff{T}"/>
		///    每个 buffed-val 可以有多个 buffId
		///    实现逻辑为（基本同VAL类）：
		///        1. buffed-val 作为 listener 注册到本地的 buffManager 上
		///        2. buffManager 在 eot-buff 每隔一段时间触发时通知 buffed-val
		///        3. buffed-val 收到通知后，调用该 buff 的 takeEffect ，基于当前值结算一次 buff 效果
		
		public enum BuffType
		{
			BUFF_ATTR,		// 附加在属性上的 buff, 只结算一次
			BUFF_VAL_EOT	// 附加在数值上的 buff, 会持续修改数值
		}

		public enum BuffId
		{
			BUFF_ATTR_STS_ATK,
			BUFF_ATTR_DAMAGE_MULTIPLIER,
			BUFF_ATTR_ACCURACY,
			BUFF_ATTR_ARMOR_FRONT,
			BUFF_ATTR_ARMOR_SIDE,
			BUFF_ATTR_ARMOR_BACK,
			BUFF_VAL_EOT_BLEED
		}
	}
}