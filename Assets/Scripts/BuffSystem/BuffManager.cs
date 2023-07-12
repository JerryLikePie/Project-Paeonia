using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Assets.Scripts.BuffSystem {
     
    // buff 系统的核心模式为 Buffee - Buff Manager - Buff
    // 即，Buffee 和 buff 通过松散的方式(BuffId)绑定到一起，由 Buff Manager 统一管理

    // Buffee 为受 buff 影响的量，例如 BuffedAttr、BuffedValue

    // Buff 为 buff 本身，从中可以派生出多种 buff 子类

    // BuffManager 为管理类，挂载在可以拥有 buff 的单位上，为 Buff 提供容器，同时接受 Buffee 作为 BuffUpdateListener 注册
    // BuffManager 可以监控所有 Buff 的状态，并按触发条件通知 Buffee 更新
    // Buffee 对自身 buff 的值的更新方式是由 Buffee 决定的(在OnBuffUpdate()中)

    /// <summary>
    /// 作为我方、敌方单位挂载的 Component
    /// 作为容器类，管理和处理单位身上的 buff
    /// </summary>
    public class BuffManager : MonoBehaviour
    {
        public LinkedList<Buff> buffs = new LinkedList<Buff>();

        public void addBuff(Buff buff)
        {
            buffs.AddLast(buff);
            if (buff.buffType == BuffConstants.BuffType.BUFF_ATTR)
            {
                invokeBuffUpdate(buff);
            }
        }

        public void removeBuff(Buff buff)
        {
            buffs.Remove(buff);
            if (buff.buffType == BuffConstants.BuffType.BUFF_ATTR)
            {
                invokeBuffUpdate(buff);
            }
        }

        public List<BuffUpdateListener> listeners = new List<BuffUpdateListener>();

        public void addListener(BuffUpdateListener listener)
        {
            listeners.Add(listener);
        }

        // 通常来说 listener 和 buffManager 的生命周期是一致的
        // 如不一致，则需要调用 removeListener 将自己移除
        public void removeListener(BuffUpdateListener listener)
        {
            listeners.Remove(listener);
        }

        /// 添加、删除 Attr Buff 时触发 <see cref="addBuff(Buff)"/><see cref="removeBuff(Buff)"/>
        /// Val EOT Buff 每次 interval 到时结算时触发 <see cref="Update"/>
        public void invokeBuffUpdate(Buff buff)
        {
            foreach (BuffUpdateListener l in listeners)
            {
                if (l == null)
                {
                    Debug.LogError("listener should not be null, check its life cycle.");
                }
                else
                {
                    if (l.interestBuffIds() == null || l.interestBuffIds().Contains(buff.buffId))
                    {
                        l.onBuffUpdate(this, buff);
                    }
                }
            }
        }

        // 计算受到所有同 buffId 的 buff 影响之后的值
        // buff 按顺序生效
        // todo: buff 结算目前是按添加顺序结算，未来有需要可以加入 priority 并使用优先队列结算
        public T takeAllEffects<T>(T val, BuffConstants.BuffId buffId)
        {
            T buffedValue = val;
            // 在该值上结算所有 buffId 相同的 buff
            foreach (Buff buff in buffs)
            {
                if (buff.buffId == buffId)
                {
                    buffedValue = (buff as ITakeEffect<T>).takeEffect(buffedValue);
                }
            }
            return buffedValue;
        }

        public void Update()
        {
            foreach (Buff buff in buffs)
            {
                // 当 eot buff 倒计时结束时，进行一次 buff 更新
                if (buff.buffType == BuffConstants.BuffType.BUFF_VAL_EOT)
                {
                    if ((buff as IBuffCD).updateCD(Time.deltaTime))
                    {
                        invokeBuffUpdate(buff);
                    }
                }
            }
		}
	}
}