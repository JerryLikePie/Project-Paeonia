using System.Collections;
using UnityEngine;

namespace Assets.Scripts.BuffSystem.BuffImpl
{
	public class DebuffBleeding : EffectOverTimeBuff<float>
	{
		private float bleedDamage;

		public DebuffBleeding(float interval, float bleedDamage) : base(BuffConstants.BuffId.BUFF_EOT_BLEED, interval)
		{
			this.bleedDamage = bleedDamage;
		}

		// 覆盖泛型方法
		public override float takeEffect(float val)
		{
			return val - bleedDamage;
		}
	}
}