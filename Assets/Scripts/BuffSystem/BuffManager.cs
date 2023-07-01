using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.BuffSystem { 
    /// <summary>
    /// 作为我方、敌方单位挂载的 Component
    /// 作为容器类，管理和处理单位身上的 buff
    /// </summary>
    public class BuffManager : MonoBehaviour
    {
        public LinkedList<Buff> buffs;

        public void addBuff(Buff buff)
		{
            buffs.AddLast(buff);
		}

        public void removeBuff(Buff buff)
		{
            buffs.Remove(buff);
		}

        // 计算受到 buff 影响之后的值
        // 按顺序生效 buff
        // todo 让 attr 缓存自己 buff 后的值，只有在 buff 变化时才重新计算
        public T takeEffects<T>(BuffedAttr<T> attr)
		{
            ValueBuff<T> aBuff;
            T buffedValue = default(T);
            // 当 BuffType 相同时对数值产生影响
            foreach (Buff buff in buffs)
			{
                if (buff.buffType == attr.buffType)
				{
                    aBuff = Convert.ChangeType(buff, typeof(ValueBuff<T>)) as ValueBuff<T>;
                    buffedValue = aBuff.takeEffect(attr.getRawValue());
				}
			}
            return buffedValue;
		}
    }
}