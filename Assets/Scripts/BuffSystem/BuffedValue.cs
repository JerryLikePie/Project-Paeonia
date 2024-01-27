using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.BuffSystem
{
	/// <summary>
	/// 用于注解实现 VAL Effect-Of-Time Buffee
	/// </summary>
	[Serializable]
	public class BuffedValue<T> : BuffUpdateListener where T : struct, IComparable
	{

		public readonly BuffConstants.BuffId[] buffIds;

		public T value;

		public BuffedValue(params BuffConstants.BuffId[] ids)
		{
			this.buffIds = ids;
			this.value = default(T);
		}

		public BuffedValue(T value, BuffConstants.BuffId[] ids)
		{
			this.buffIds = ids;
			this.value = value;
		}

		public void setValue(T val)
		{
			this.value = val;
		}

		public T getValue()
		{
			return value;
		}

		public void registToBuffManager(BuffManager buffManager)
		{
			buffManager.addListener(this);
		}

		public HashSet<BuffConstants.BuffId> interestBuffIds()
		{
			return new HashSet<BuffConstants.BuffId>(buffIds);
		}

		public void onBuffUpdate(BuffManager buffManager, Buff buff)
		{
			value = (buff as ITakeEffect<T>).takeEffect(value);
		}

		public override bool Equals(object obj)
		{
			return obj is BuffedValue<T> value &&
				   buffIds.Equals(value.buffIds); 
		}

		public override int GetHashCode()
		{
			return -747130871 + buffIds.GetHashCode();
		}

		public override string ToString()
		{
			return value.ToString();
		}

		public static T operator +(BuffedValue<T> me, T plus)
		{
			return Utilities.Add(me.value, plus);
		}

		public static T operator -(BuffedValue<T> me, T minus)
		{
			return Utilities.Sub(me.value, minus);
		}

		public static T operator *(BuffedValue<T> me, T mul)
		{
			return Utilities.Multiply(me.value, mul);
		}

		public static T operator /(BuffedValue<T> me, T div)
		{
			return Utilities.Divide(me.value, div);
		}

		public static bool operator >=(BuffedValue<T> me, T other)
		{
			return me.value.CompareTo(other) >= 0;
		}

		public static bool operator <=(BuffedValue<T> me, T other)
		{
			return me.value.CompareTo(other) <= 0;
		}
		public static bool operator >(BuffedValue<T> me, T other)
		{
			return me.value.CompareTo(other) > 0;
		}

		public static bool operator <(BuffedValue<T> me, T other)
		{
			return me.value.CompareTo(other) < 0;
		}

		public static bool operator ==(BuffedValue<T> me, T other)
		{
			return me.value.CompareTo(other) == 0;
		}

		public static bool operator !=(BuffedValue<T> me, T other)
		{
			return me.value.CompareTo(other) != 0;
		}
	}
}