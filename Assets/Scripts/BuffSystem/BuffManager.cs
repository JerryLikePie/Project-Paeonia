using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.BuffSystem { 
    /// <summary>
    /// ��Ϊ�ҷ����з���λ���ص� Component
    /// ��Ϊ�����࣬����ʹ���λ���ϵ� buff
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

        // �����ܵ� buff Ӱ��֮���ֵ
        // ��˳����Ч buff
        // todo �� attr �����Լ� buff ���ֵ��ֻ���� buff �仯ʱ�����¼���
        public T takeEffects<T>(BuffedAttr<T> attr)
		{
            ValueBuff<T> aBuff;
            T buffedValue = default(T);
            // �� BuffType ��ͬʱ����ֵ����Ӱ��
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