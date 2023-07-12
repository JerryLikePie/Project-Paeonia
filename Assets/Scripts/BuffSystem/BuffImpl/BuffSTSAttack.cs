using System.Collections;
using UnityEngine;

namespace Assets.Scripts.BuffSystem.BuffImpl
{
	public class BuffSTSAttack : AttrBuff<float>
	{
		public BuffSTSAttack() : base(BuffConstants.BuffId.BUFF_ATTR_STS_ATK)
		{

		}

		public override float takeEffect(float val)
		{
			return val + 50;
		}
	}
}