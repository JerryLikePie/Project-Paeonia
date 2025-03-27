using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utilities;

public class EnemyResource : IEnemyBehavior
{
    private DollsCombat doll;
    // ����
    // ��������˲����������ƶ���������������Ŀ�꣬���ǿ��Ա���ը���ֶ�����Ӱ�졣��������ʱ�����95%�ļ��ˡ�
    // ���ҷ���λվ�ڿ����൥λ���ڸ���ʱ����������˿�ʼ���ҷֽ⣨ÿ���ܵ�1���˺�������ֵ20��
    // �����൥λ����ʱ������һ�����Ŀ��������������dolls�ı�����ͬʱ��ͼ���ӵ�ͬ�ڿ������������ֵ�Ļ���ֵ
    public override void CheckDolls(EnemyCombat context)
    {
        try
        {
            for (int i = 0; i < context.dollsList.transform.childCount; i++)
            {
                doll = context.dollsList.transform.GetChild(i).GetComponent<DollsCombat>();
                if (doll == null)
                {
                    continue;
                }
                if (doll.getType() == 3)
                {
                    // ����ǿվ��Ļ�����
                    continue;
                }
                if (FindDistance(transform.gameObject, doll.gameObject) <= 17.5f * 1.2f)
                {
                    if (doll.gameObject.activeSelf)
                    {
                        //��ʼ��Ѫ
                        context.RecieveDamage(0.01f);
                    }
                }
            }
        }
        catch
        {

        }
    }
}
