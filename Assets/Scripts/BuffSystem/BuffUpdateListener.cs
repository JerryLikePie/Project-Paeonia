using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.BuffSystem
{
	/// <summary>
	/// 实现 Buff 状态更新通知的接口
	/// 
	/// 目前 Buff 系统的更新逻辑设计为：
	///    buffManager 在满足条件时通知更新
	///    -> Listener 收到更新
	///    -> Listener 调用 buffManager 的方法修改自身状态
	/// </summary>
	public interface BuffUpdateListener
	{
		// 要 listen 的 buffId 集合，返回 null 表示监听所有
		public abstract HashSet<BuffConstants.BuffId> interestBuffIds();

		// 当某个 buff 发生更新时调用
		public abstract void onBuffUpdate(BuffManager buffManager, Buff buff);
	}
}