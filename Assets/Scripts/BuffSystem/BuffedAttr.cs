using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.BuffSystem
{
	/// <summary>
	/// 用于标识受到 buff 的属性
	/// 在每次 getBuffedValue 时返回 buff 后的属性值
	/// </summary>
	/// <typeparam name="T">内部数值类型，通常为 int float</typeparam>
	public class BuffedAttr<T>
	{

		public BuffConstants.BuffType buffType { get; }

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

		// 绑定 BuffMangaer
		public BuffedAttr(BuffConstants.BuffType type)
		{
			this.buffType = type;
		}

		public BuffedAttr(BuffConstants.BuffType type, T value)
		{
			this.buffType = type;
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
				return buffManager.takeEffects(this);
			}
			else
			{
				Debug.LogError("在获取被 buff 的数值时出现错误，buffManager 为 null");
				return default(T);
			}
		}
	}
}