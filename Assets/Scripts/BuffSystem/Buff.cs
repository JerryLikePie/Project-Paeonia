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
		public BuffConstants.BuffType buffType { get; }

		public BuffConstants.BuffId buffId { get; }

		public Buff(BuffConstants.BuffType type, BuffConstants.BuffId id)
		{
			this.buffType = type;
			this.buffId = id;
		}

	}

	// todo 用组合代替继承！
	public interface ITakeEffect<T>
	{
		public abstract T takeEffect(T val);
	}

	public abstract class ValueBuff<T> : Buff
	{
		public ValueBuff(BuffConstants.BuffId id) : base(BuffConstants.BuffType.BUFF_VAL, id)
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
	public class EffectOverTimeBuff<T> : Buff
	{
		// buff 触发时间间隔
		public float interval;

		// buff 触发倒计时
		public float cd;

		public EffectOverTimeBuff(BuffConstants.BuffId id, float interval) : base(BuffConstants.BuffType.BUFF_EOT, id)
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
			if (cd <= 0) resetCD();
			return cd <= 0;
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
	public class ValueBuffD<T> : ValueBuff<T>
	{
		public new TakeEffectDelegate takeEffect;

		public ValueBuffD(BuffConstants.BuffId id, TakeEffectDelegate effectFunc) : base(id)
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
		public static ValueBuffD<T> createSimpleAdditionBuff<T>(BuffConstants.BuffId id, T inc) where T : struct, IComparable
		{ 
			return new ValueBuffD<T>(id, (T value) =>
			{
				return Add(value, inc);
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
		public static ValueBuffD<T> createSimpleMultiplicationBuff<T>(BuffConstants.BuffId id, T mux) where T : struct, IComparable
		{
			return new ValueBuffD<T>(id, (T value) =>
			{
				return Multiply(value, mux);
			});
		}

		private static T Add<T>(T num1, T num2) where T : struct, IComparable
		{
			if (typeof(T) == typeof(short))
			{
				short a = (short)Convert.ChangeType(num1, typeof(short));
				short b = (short)Convert.ChangeType(num2, typeof(short));
				short c = (short)(a + b);
				return (T)Convert.ChangeType(c, typeof(T));
			}
			else if (typeof(T) == typeof(int))
			{
				int a = (int)Convert.ChangeType(num1, typeof(int));
				int b = (int)Convert.ChangeType(num2, typeof(int));
				int c = a + b;
				return (T)Convert.ChangeType(c, typeof(T));
			}
			else if (typeof(T) == typeof(long))
			{
				long a = (long)Convert.ChangeType(num1, typeof(long));
				long b = (long)Convert.ChangeType(num2, typeof(long));
				long c = a + b;
				return (T)Convert.ChangeType(c, typeof(T));
			}
			else if (typeof(T) == typeof(float))
			{
				float a = (float)Convert.ChangeType(num1, typeof(float));
				float b = (float)Convert.ChangeType(num2, typeof(float));
				float c = a + b;
				return (T)Convert.ChangeType(c, typeof(T));
			}
			else if (typeof(T) == typeof(double))
			{
				double a = (double)Convert.ChangeType(num1, typeof(double));
				double b = (double)Convert.ChangeType(num2, typeof(double));
				double c = a + b;
				return (T)Convert.ChangeType(c, typeof(T));
			}
			else if (typeof(T) == typeof(decimal))
			{
				decimal a = (decimal)Convert.ChangeType(num1, typeof(decimal));
				decimal b = (decimal)Convert.ChangeType(num2, typeof(decimal));
				decimal c = a + b;
				return (T)Convert.ChangeType(c, typeof(T));
			}

			return default(T);
		}
		private static T Multiply<T>(T num1, T num2) where T : struct, IComparable
		{
			if (typeof(T) == typeof(int))
			{
				int a = (int)Convert.ChangeType(num1, typeof(int));
				int b = (int)Convert.ChangeType(num2, typeof(int));
				int c = a * b;
				return (T)Convert.ChangeType(c, typeof(T));
			}
			else if (typeof(T) == typeof(float))
			{
				float a = (float)Convert.ChangeType(num1, typeof(float));
				float b = (float)Convert.ChangeType(num2, typeof(float));
				float c = a * b;
				return (T)Convert.ChangeType(c, typeof(T));
			}
			else if (typeof(T) == typeof(double))
			{
				double a = (double)Convert.ChangeType(num1, typeof(double));
				double b = (double)Convert.ChangeType(num2, typeof(double));
				double c = a * b;
				return (T)Convert.ChangeType(c, typeof(T));
			}
			else if (typeof(T) == typeof(decimal))
			{
				decimal a = (decimal)Convert.ChangeType(num1, typeof(decimal));
				decimal b = (decimal)Convert.ChangeType(num2, typeof(decimal));
				decimal c = a * b;
				return (T)Convert.ChangeType(c, typeof(T));
			}

			return default(T);
		}
	}
}