using System;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.BuffSystem
{
	// 所有 buff 的基类，buff 相关的全局属性可以放到这里

	// BuffType 用来区分不同处理方式的 Buff
	// BuffId 相当于一个频道，挂在这个频道上的 被Buff量 会受到同频道 Buff 影响
	public abstract class Buff
	{
		public readonly BuffConstants.BuffType buffType;

		public readonly BuffConstants.BuffId buffId;

		public Buff(BuffConstants.BuffType type, BuffConstants.BuffId id)
		{
			this.buffType = type;
			this.buffId = id;
		}

	}

	// 用组合代替继承！
	// buff 效果结算函数
	public interface ITakeEffect<T>
	{
		public abstract T takeEffect(T val);
	}

	// 用于需要设置和重置 cd 的 buff，例如 EOT-buff
	public interface IBuffCD
	{
		public abstract void resetCD();

		public abstract bool updateCD(float deltaTime);
	}


	public abstract class AttrBuff<T> : Buff, ITakeEffect<T>
	{
		public AttrBuff(BuffConstants.BuffId id) : base(BuffConstants.BuffType.BUFF_ATTR, id)
		{
			// no-op
		}

		public virtual T takeEffect(T val)
		{
			Debug.LogError("not this takeEffect!");
			return default(T);
		}
	}

	/// <summary>
	/// 每过一段指定时间执行 takeEffect 函数
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class EffectOverTimeBuff<T> : Buff, ITakeEffect<T>, IBuffCD
	{
		// buff 触发时间间隔
		public float interval;

		// buff 触发倒计时
		public float cd;

		public EffectOverTimeBuff(BuffConstants.BuffId id, float interval) : base(BuffConstants.BuffType.BUFF_VAL_EOT, id)
		{
			this.interval = interval;
			// 是否在一开始重置 cd 决定了挂上 buff 的瞬间是否要触发一次 buff 效果
			this.cd = interval;
		}

		// 重置触发cd
		public void resetCD()
		{
			cd = interval;
		}

		public bool updateCD(float deltaTime)
		{
			cd -= deltaTime;
			bool updated = (cd <= 0);
			if (updated) resetCD();
			return updated;
		}

		// 对要修改的 value 产生影响的函数
		public virtual T takeEffect(T val)
		{
			return default(T);
		}

	}

	// 影响数值的 Buff 子类
	/// <summary>
	/// 使用代理方法实现 takeEffect 函数，似乎暂时用处不大
	/// </summary>
	/// <typeparam name="T"></typeparam>
	// todo Obsolete
	[Obsolete]
	public class AttrBuffD<T> : AttrBuff<T>
	{
		public new TakeEffectDelegate takeEffect;

		public AttrBuffD(BuffConstants.BuffId id, TakeEffectDelegate effectFunc) : base(id)
		{
			takeEffect = effectFunc;
		}

		// 对原始 value 产生影响的函数
		public delegate T TakeEffectDelegate(T value);
	}


	public static class BuffFactory
	{
		// todo Obsolete
		[Obsolete]
		/// <summary>
		/// 生成一个加算数值型 Buff，可以实现数值的加减
		/// 好像用处不是很大（
		/// 直接继承重写一个就挺好
		/// </summary>
		/// <typeparam name="T">数值的数据类型</typeparam>
		/// <param name="buffType">buff类型</param>
		/// <param name="inc">数值增量</param>
		/// <returns>生成的 Buff</returns>
		public static AttrBuffD<T> createSimpleAdditionBuff<T>(BuffConstants.BuffId id, T inc) where T : struct, IComparable
		{ 
			return new AttrBuffD<T>(id, (T value) =>
			{
				return Utilities.Add(value, inc);
			});
		}

		// todo Obsolete
		[Obsolete]
		/// <summary>
		/// 生成一个乘算数值型 Buff，可以实现数值的乘除 
		/// </summary>
		/// <typeparam name="T">数值的数据类型</typeparam>
		/// <param name="buffType">buff类型</param>
		/// <param name="mux">数值乘子</param>
		/// <returns>生成的 Buff</returns>
		public static AttrBuffD<T> createSimpleMultiplicationBuff<T>(BuffConstants.BuffId id, T mux) where T : struct, IComparable
		{
			return new AttrBuffD<T>(id, (T value) =>
			{
				return Utilities.Multiply(value, mux);
			});
		}

	}
}