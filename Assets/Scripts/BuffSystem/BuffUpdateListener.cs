using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.BuffSystem
{
	public interface BuffUpdateListener
	{
		// 要 listen 的 buffId 集合，返回 null 表示监听所有
		public abstract HashSet<BuffConstants.BuffId> interestBuffIds();

		// 当某个 buff 发生更新时调用
		public abstract void onBuffUpdate(BuffConstants.BuffId buffId);
	}
}