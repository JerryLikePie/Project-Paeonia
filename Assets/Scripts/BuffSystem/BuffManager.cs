using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Assets.Scripts.BuffSystem { 
    /// <summary>
    /// ��Ϊ�ҷ����з���λ���ص� Component
    /// ��Ϊ�����࣬����ʹ���λ���ϵ� buff
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

        // ��ӡ�ɾ����ֵ�� Buff ʱ�����¼���ͬ BuffId ����ֵ
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

        // �����ܵ� buff Ӱ��֮���ֵ
        // ��˳����Ч buff
        // todo buff ����Ŀǰ�ǰ����˳����㣬δ������Ҫ���Լ��� priority ��ʹ�����ȶ��н���
        public T takeEffects<T>(T val, BuffConstants.BuffId buffId)
		{
            ValueBuff<T> aBuff;
            T buffedValue = val;
            // �ڸ�ֵ�Ͻ������� buffId ��ͬ�� buff
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
                // todo ���� VAL �� buff
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