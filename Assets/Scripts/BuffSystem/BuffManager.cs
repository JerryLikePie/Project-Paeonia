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

        // ��ӡ�ɾ����ֵ�� Buff ʱ���������¼���ͬ BuffId ����ֵ
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

        // �����ܵ� buff Ӱ��֮���ֵ
        // ��˳����Ч buff
        // todo buff ����Ŀǰ�ǰ����˳����㣬δ������Ҫ���Լ��� priority ��ʹ�����ȶ��н���
        public T takeEffects<T>(T val, BuffConstants.BuffId buffId)
        {
            T buffedValue = val;
            // �ڸ�ֵ�Ͻ������� buffId ��ͬ�� buff
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
        /// �����ȡ clazz �������� VAL Buff Ӱ����ֶ�
        /// </summary>
        /// <param name="clazz">Ŀ����</param>
        /// <returns></returns>
        public static Dictionary<BuffConstants.BuffId, FieldInfo> getBuffedFields(Type clazz)
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

        /// <summary>
        /// �����ȡ clazz �������� VAL Buff Ӱ��ģ�ָ�� buffType ���ֶ�
        /// </summary>
        /// <param name="clazz">Ŀ����</param>
        /// <param name="buffTypeSelector">���� buffType ����</param>
        /// <returns>�ܵ� VAL Buff Ӱ����ֶ�</returns>
        public static Dictionary<BuffConstants.BuffId, FieldInfo> getBuffedFields(Type clazz, BuffConstants.BuffType buffTypeSelector)
        {
            Dictionary<BuffConstants.BuffId, FieldInfo> buffedFields = new Dictionary<BuffConstants.BuffId, FieldInfo>();

            var fields = clazz.GetFields();
            foreach (var f in fields)
            {
                // todo ���� VAL �� buff
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
                // �� eot buff ����ʱ����ʱ������һ�� buff ����
                if (buff.buffType == BuffConstants.BuffType.BUFF_EOT)
                {
                    checkMethod = buff.GetType().GetMethod("updateCD");
                    
                    invokeBuffUpdate(buff);
                }
            }
		}
	}
}