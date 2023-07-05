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

	// 影响数值的 Buff 子类
	// todo 移出到独立的文件中
	public class ValueBuff<T> : Buff 
	{
		public TakeEffectDelegate takeEffect;

		public ValueBuff(BuffConstants.BuffId id, TakeEffectDelegate effectFunc) : base(BuffConstants.BuffType.BUFF_VAL, id)
		{
			takeEffect = effectFunc;
		}

		// 对原始 value 产生影响的函数
		public delegate T TakeEffectDelegate(T value);
	}

	public class EffectOverTimeBuff : Buff
	{
		public EffectOverTimeBuff(BuffConstants.BuffId id) : base(BuffConstants.BuffType.BUFF_EOT, id)
		{
		}


	}

	public static class BuffFactory
	{

		/// <summary>
		/// 生成一个加算数值型 Buff，可以实现数值的加减
		/// </summary>
		/// <typeparam name="T">数值的数据类型</typeparam>
		/// <param name="buffType">buff类型</param>
		/// <param name="inc">数值增量</param>
		/// <returns>生成的 Buff</returns>
		public static ValueBuff<T> createAdditionBuff<T>(BuffConstants.BuffId id, T inc) where T : struct, IComparable
		{ 
			return new ValueBuff<T>(id, (T value) =>
			{
				return Add(value, inc);
			});
		}

		/// <summary>
		/// 生成一个乘算数值型 Buff，可以实现数值的乘除 
		/// </summary>
		/// <typeparam name="T">数值的数据类型</typeparam>
		/// <param name="buffType">buff类型</param>
		/// <param name="inc">数值增量</param>
		/// <returns>生成的 Buff</returns>
		public static ValueBuff<T> createMultiplicationBuff<T>(BuffConstants.BuffId id, T inc) where T : struct, IComparable
		{
			return new ValueBuff<T>(id, (T value) =>
			{
				return Multiply(value, inc);
			});
		}

		private static T Add<T>(T num1, T num2) where T : struct, IComparable
		{
			if (typeof(T) == typeof(int))
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