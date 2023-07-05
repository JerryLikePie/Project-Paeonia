using System;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.BuffSystem
{
	// todo AllowMultiple=true
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class BuffedAttrAttribute : Attribute
	{
		// private Type attrType;
		public BuffConstants.BuffType buffType;
		public BuffConstants.BuffId buffId;
		public BuffedAttrAttribute(BuffConstants.BuffType buffType, BuffConstants.BuffId buffId)
		{
			// this.attrType = attrType;
			this.buffType = buffType;
			this.buffId = buffId;
		}
	}

	/// <summary>
	/// 用于标识受到 buff 的属性
	/// 在每次 getBuffedValue 时返回 buff 后的属性值
	/// todo 暂时废弃，未来可能另作他用
	/// </summary>
	/// <typeparam name="T">内部数值类型，通常为 int float</typeparam>
	public class BuffedAttr<T>
	{

		public BuffConstants.BuffId buffId { get; }

		private T value
		{
			get
			{
				return value;
			}
			set
			{
				this.value = value;
			}
		}

		/// <summary>
		/// 创建受Buff影响的属性
		/// </summary>
		/// <param name="id">该属性会受到何种buff影响</param>
		public BuffedAttr(BuffConstants.BuffId id)
		{
			this.buffId = id;
		}

		/// <summary>
		/// 创建受Buff影响的属性
		/// </summary>
		/// <param name="id">该属性会受到何种buff影响</param>
		/// <param name="value">该属性的初始值</param>
		public BuffedAttr(BuffConstants.BuffId id, T value)
		{
			this.buffId = id;
			this.value = value;
		}
		
		public T getRawValue()
		{
			return value;
		}

		public T getBuffedValue(BuffManager buffManager)
		{
			if (buffManager != null)
			{
				return buffManager.takeEffects(value, buffId);
			}
			else
			{
				Debug.LogError("在获取被 buff 的数值时出现错误，buffManager 为 null");
				return default(T);
			}
		}
	}
}