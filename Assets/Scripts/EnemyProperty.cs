using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProperty : MonoBehaviour
{
    public string enemy_name;//����
    public int enemy_id;//ID
    public int enemy_type;//1½�� 2֧Ԯ 3�վ� 4����
    public int enemy_range;//��������
    public float enemy_max_hp;//�������ֵ
    public float[] enemy_damage_recieved_multiplier;//��ͬ���ֵ��˺���һ��

    public float enemy_penetration;//����
    public float enemy_sts_attack;//�ضԵع�����
    public float enemy_ata_attack;//�նԿչ�����
    public float enemy_sta_attack;//�ضԿչ�����
    public float enemy_ats_attack;//�նԵع�����

    public int enemy_accuracy;//����
    public int enemy_dodge;//����
    public float enemy_reload;//װ��
    public float enemy_firerate;//������ǵ����ڣ���ô�������ʱ��͵���װ��ʱ��

    public float enemy_armor_front;//ǰװ��
    public int enemy_armor_side;//��װ��
    public int enemy_armor_back;//��װ��
    public float enemy_damage_multiplier;//�˺�����
    public int enemy_ammo_type;//���� 0���׵� 1�߱��� 2�Ƽ׵� 3APCBC�Ⱥ�Ч��

    public bool enemy_airstrike;//�ɷ��Ϯ
    public bool enemy_recon;//�ɷ����
    public bool enemy_air_sup;//�ɷ��ƿ�
    public bool enemy_artillery;//�ɷ��ڻ�
    public float enemy_skill_point;//���ܵ���
    public float enemy_skill_fullcharge;//�����������
    public float enemy_moveTime;

    public bool enemy_withdrew;//�Ƿ��³�
    public bool enemy_visible = false;//�Ƿ�ɼ�

    public int enemy_mag;//����������������ǵ������Ǿ���1��
    public int enemy_shell_speed;//����
}
