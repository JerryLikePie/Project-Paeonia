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
            invokeBuffUpdate(buff);
		}

        public void removeBuff(Buff buff)
		{
            buffs.Remove(buff);
            invokeBuffUpdate(buff);
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

        // 添加、删除数值型 Buff 时，重新计算同 BuffId 的数值
        public void invokeBuffUpdate(Buff buff)
        {
            if (buff.buffType == BuffConstants.BuffType.BUFF_VAL)
            {
                foreach (Buff b in buffs)
				{
                    if (b.buffId == buff.buffId)
					{
                        foreach (BuffUpdateListener l in listeners)
						{
                            if (l.interestBuffIds() == null)
							{
                                l.onBuffUpdate(buff.buffId);
							}
                            else if (l.interestBuffIds().Contains(buff.buffId))
							{
                                l.onBuffUpdate(buff.buffId);
							}
						}
					}
				}
            }
        }


        public void Start()
		{

		}

        // 计算受到 buff 影响之后的值
        // 按顺序生效 buff
        // todo buff 结算目前是按添加顺序结算，未来有需要可以加入 priority 并使用优先队列结算
        public T takeEffects<T>(T val, BuffConstants.BuffId buffId)
		{
            ValueBuff<T> aBuff;
            T buffedValue = val;
            // 在该值上结算所有 buffId 相同的 buff
            foreach (Buff buff in buffs)
			{
                if (buff.buffType == BuffConstants.BuffType.BUFF_VAL && buff.buffId == buffId)
				{
                    aBuff = buff as ValueBuff<T>;
                    buffedValue = aBuff.takeEffect(buffedValue);
				}
			}
            return buffedValue;
		}

        public static Dictionary<BuffConstants.BuffId, FieldInfo> getValueBuffedFields(Type clazz)
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
    }
}