using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Assets.Scripts.BuffSystem {
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
            if (buff.buffType == BuffConstants.BuffType.BUFF_VAL)
            {
                invokeBuffUpdate(buff);
            }
        }

        public void removeBuff(Buff buff)
        {
            buffs.Remove(buff);
            if (buff.buffType == BuffConstants.BuffType.BUFF_VAL)
            {
                invokeBuffUpdate(buff);
            }
        }

        public List<BuffUpdateListener> listeners = new List<BuffUpdateListener>();

        public void addListener(BuffUpdateListener listener)
        {
            listeners.Add(listener);
        }

        public void removeListener(BuffUpdateListener listener)
        {
            listeners.Remove(listener);
        }

        // 添加、删除数值型 Buff 时，触发重新计算同 BuffId 的数值
        // 
        public void invokeBuffUpdate(Buff buff)
        {
            foreach (BuffUpdateListener l in listeners)
            {
                if (l.interestBuffIds() == null || l.interestBuffIds().Contains(buff.buffId))
                {
                    l.onBuffUpdate(buff.buffId);
                }
            }
        }

        // 计算受到 buff 影响之后的值
        // 按顺序生效 buff
        // todo buff 结算目前是按添加顺序结算，未来有需要可以加入 priority 并使用优先队列结算
        public T takeEffects<T>(T val, BuffConstants.BuffId buffId)
        {
            T buffedValue = val;
            // 在该值上结算所有 buffId 相同的 buff
            foreach (Buff buff in buffs)
            {
                if (buff.buffType == BuffConstants.BuffType.BUFF_VAL && buff.buffId == buffId)
                {
                    buffedValue = (buff as ValueBuff<T>).takeEffect(buffedValue);
                }
            }
            return buffedValue;
        }

        /// <summary>
        /// 反射获取 clazz 类所有受 VAL Buff 影响的字段
        /// </summary>
        /// <param name="clazz">目标类</param>
        /// <returns></returns>
        public static Dictionary<BuffConstants.BuffId, FieldInfo> getBuffedFields(Type clazz)
        {
            Dictionary<BuffConstants.BuffId, FieldInfo> buffedFields = new Dictionary<BuffConstants.BuffId, FieldInfo>();

            var fields = clazz.GetFields();
            foreach (var f in fields)
            {
                // todo 过滤 VAL 型 buff
                var annos = f.GetCustomAttributes<BuffedAttrAttribute>(false);
                foreach (var anno in annos)
                {
                    buffedFields.Add(anno.buffId, f);
                }
            }

            return buffedFields;
        }

        /// <summary>
        /// 反射获取 clazz 类所有受 VAL Buff 影响的，指定 buffType 的字段
        /// </summary>
        /// <param name="clazz">目标类</param>
        /// <param name="buffTypeSelector">过滤 buffType 类型</param>
        /// <returns>受到 VAL Buff 影响的字段</returns>
        public static Dictionary<BuffConstants.BuffId, FieldInfo> getBuffedFields(Type clazz, BuffConstants.BuffType buffTypeSelector)
        {
            Dictionary<BuffConstants.BuffId, FieldInfo> buffedFields = new Dictionary<BuffConstants.BuffId, FieldInfo>();

            var fields = clazz.GetFields();
            foreach (var f in fields)
            {
                // todo 过滤 VAL 型 buff
                var annos = f.GetCustomAttributes<BuffedAttrAttribute>(false);
                foreach (var anno in annos)
                {
                    if (anno.buffType == buffTypeSelector)
                    {
                        buffedFields.Add(anno.buffId, f);
                    }
                }
            }

            return buffedFields;
        }


        public void Update()
        {
            MethodInfo checkMethod;
            foreach (Buff buff in buffs)
            {
                // 当 eot buff 倒计时结束时，进行一次 buff 更新
                if (buff.buffType == BuffConstants.BuffType.BUFF_EOT)
                {
                    checkMethod = buff.GetType().GetMethod("updateCD");
                    
                    invokeBuffUpdate(buff);
                }
            }
		}
	}
}