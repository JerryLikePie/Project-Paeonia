using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DollsProperty : MonoBehaviour
{
    public string dolls_name;//����
    public int dolls_ammount;//�м�����֮����Լ�����������֮����趨����Ȼ����֪��զ���ֳ���
    public int dolls_star;//�Ǽ�
    public int dolls_id;//ID
    public int dolls_type;//1½�� 2֧Ԯ 3�վ� 4����
    public int dolls_range;//��������
    public int dolls_view_range;//��Ұ
    public bool dolls_unlocked;//�Ƿ��ѽ���
    public float dolls_max_hp;//�������ֵ
    public float dolls_sts_attack;//�ضԵع�����
    public float dolls_ata_attack;//�նԿչ�����
    public float dolls_sta_attack;//�ضԿչ�����
    public float dolls_ats_attack;//�նԵع�����
    public float dolls_penetration;//����
    public int dolls_accuracy;//����
    public int dolls_dodge;//����

    public float dolls_reload;//װ��
    public float dolls_firerate;//������ǵ����ڣ���ô�������ʱ��͵���װ��ʱ��

    public float dolls_armor_front;//ǰװ��
    public int dolls_armor_side;//��װ��
    public int dolls_armor_back;//��װ��

    public float dolls_damage_multiplier;//�˺�����
    public int dolls_ammo_type;//���� 0���׵� 1�߱��� 2�Ƽ׵� 3APCBC�Ⱥ�Ч�� 4����ը�� 5β���ȶ��ѿǴ��׵�

    public float dolls_skill_point;//���ܵ���
    public float dolls_skill_fullcharge;//�����������

    public bool dolls_withdrew;//�Ƿ��³�

    public int dolls_mag;//����������������ǵ������Ǿ���1��
    public int dolls_shell_speed;//����
    public int dolls_mag2;//����и������Ļ�

    float storeAttack;
    float storeReload;
    float storePen;

    void Start()
    {
        storeAttack = dolls_sts_attack;
        storeReload = dolls_reload;
        storePen = dolls_penetration;
    }

    public void Buff(float attackBuff, float reloadBuff, float penetrationBuff, float time)
    {
        dolls_sts_attack += attackBuff;
        dolls_reload += reloadBuff;
        dolls_penetration += penetrationBuff;
        Invoke("RemoveAtkBuff", time);
        Invoke("RemoveSpdBuff", time);
        Invoke("RemovePenBuff", time);
    }
    public void SwitchAmmo(int newAmmoType)
    {
        dolls_ammo_type = newAmmoType;
    }
    public void RemoveAtkBuff()
    {
        dolls_sts_attack = storeAttack;
    }
    public void RemoveSpdBuff()
    {
        dolls_reload = storeReload;
    }
    public void RemovePenBuff()
    {
        dolls_penetration = storePen;
    }
}
