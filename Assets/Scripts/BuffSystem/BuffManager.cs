using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Assets.Scripts.BuffSystem {
     
    // buff ϵͳ�ĺ���ģʽΪ Buffee - Buff Manager - Buff
    // ����Buffee �� buff ͨ����ɢ�ķ�ʽ(BuffId)�󶨵�һ���� Buff Manager ͳһ����

    // Buffee Ϊ�� buff Ӱ����������� BuffedAttr��BuffedValue

    // Buff Ϊ buff �������п������������� buff ����

    // BuffManager Ϊ�����࣬�����ڿ���ӵ�� buff �ĵ�λ�ϣ�Ϊ Buff �ṩ������ͬʱ���� Buffee ��Ϊ BuffUpdateListener ע��
    // BuffManager ���Լ������ Buff ��״̬��������������֪ͨ Buffee ����
    // Buffee ������ buff ��ֵ�ĸ��·�ʽ���� Buffee ������(��OnBuffUpdate()��)

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

        // ͨ����˵ listener �� buffManager ������������һ�µ�
        // �粻һ�£�����Ҫ���� removeListener ���Լ��Ƴ�
        public void removeListener(BuffUpdateListener listener)
        {
            listeners.Remove(listener);
        }

        /// ��ӡ�ɾ�� Attr Buff ʱ���� <see cref="addBuff(Buff)"/><see cref="removeBuff(Buff)"/>
        /// Val EOT Buff ÿ�� interval ��ʱ����ʱ���� <see cref="Update"/>
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

        // �����ܵ�����ͬ buffId �� buff Ӱ��֮���ֵ
        // buff ��˳����Ч
        // todo: buff ����Ŀǰ�ǰ����˳����㣬δ������Ҫ���Լ��� priority ��ʹ�����ȶ��н���
        public T takeAllEffects<T>(T val, BuffConstants.BuffId buffId)
        {
            T buffedValue = val;
            // �ڸ�ֵ�Ͻ������� buffId ��ͬ�� buff
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
                // �� eot buff ����ʱ����ʱ������һ�� buff ����
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