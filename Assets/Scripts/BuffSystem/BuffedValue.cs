using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.BuffSystem
{
	[Serializable]
	public class BuffedValue<T> : BuffUpdateListener where T : struct, IComparable
	{

		public readonly BuffConstants.BuffId buffId;

		public T value;

		public BuffedValue(BuffConstants.BuffId id)
		{
			this.buffId = id;
			this.value = default(T);
		}

		public BuffedValue(BuffConstants.BuffId id, T value)
		{
			this.buffId = id;
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
			return new HashSet<BuffConstants.BuffId>(new BuffConstants.BuffId[] { buffId });
		}

		public void onBuffUpdate(BuffManager buffManager, Buff buff)
		{
			value = buffManager.takeEffects<T>(value, buff.buffId);
		}

		public override bool Equals(object obj)
		{
			return obj is BuffedValue<T> value &&
				   buffId == value.buffId;
		}

		public override int GetHashCode()
		{
			return -747130871 + buffId.GetHashCode();
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
			return Utilities.Multiply(me.value, div);
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