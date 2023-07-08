using System.Collections;
using UnityEngine;

namespace Assets.Scripts.BuffSystem.BuffImpl
{
	public class BuffSTSAttack : ValueBuff<float>
	{
		public BuffSTSAttack() : base(BuffConstants.BuffId.BUFF_VAL_ATK)
		{

		}

		public override float takeEffect(float val)
		{
			return val + 10;
		}
	}
}