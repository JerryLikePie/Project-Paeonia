using Assets.Scripts.BuffSystem;
using UnityEngine;

namespace Assets.Scripts.BuffSystem.BuffImpl
{
    /// <summary>
    /// 将装甲归零的 debuff
    /// </summary>
    public class BuffArmorZeroFloat : AttrBuff<float>
    {
        public BuffArmorZeroFloat(BuffConstants.BuffId id) : base(id) { }

        public override float takeEffect(float val)
        {
            return 0f;
        }
    }

    /// <summary>
    /// 将装甲归零的 debuff（int）
    /// </summary>
    public class BuffArmorZeroInt : AttrBuff<int>
    {
        public BuffArmorZeroInt(BuffConstants.BuffId id) : base(id) { }

        public override int takeEffect(int val)
        {
            return 0;
        }
    }

    /// <summary>
    /// 乘算伤害倍率
    /// </summary>
    public class BuffDamageMultiplier : AttrBuff<float>
    {
        readonly float multiplier;

        public BuffDamageMultiplier(float multiplier) : base(BuffConstants.BuffId.BUFF_ATTR_DAMAGE_MULTIPLIER)
        {
            this.multiplier = multiplier;
        }

        public override float takeEffect(float val)
        {
            return val * multiplier;
        }
    }

    /// <summary>
    /// 乘算命中
    /// </summary>
    public class BuffAccuracyMultiplier : AttrBuff<int>
    {
        readonly float multiplier;

        public BuffAccuracyMultiplier(float multiplier) : base(BuffConstants.BuffId.BUFF_ATTR_ACCURACY)
        {
            this.multiplier = multiplier;
        }

        public override int takeEffect(int val)
        {
            return Mathf.RoundToInt(val * multiplier);
        }
    }
}

