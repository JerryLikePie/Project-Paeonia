using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProperty : MonoBehaviour
{
    public string enemy_name;//名称
    public int enemy_id;//ID
    public int enemy_type;//1陆军 2支援 3空军 4防空
    public int enemy_range;//攻击距离
    public float enemy_max_hp;//最大生命值
    public float[] enemy_damage_recieved_multiplier;//不同弹种的伤害不一样

    public float enemy_penetration;//穿深
    public float enemy_sts_attack;//地对地攻击力
    public float enemy_ata_attack;//空对空攻击力
    public float enemy_sta_attack;//地对空攻击力
    public float enemy_ats_attack;//空对地攻击力

    public int enemy_accuracy;//命中
    public int enemy_dodge;//闪避
    public float enemy_reload;//装填
    public float enemy_firerate;//如果不是弹夹炮，那么这个开火时间就等于装填时间

    public float enemy_armor_front;//前装甲
    public int enemy_armor_side;//侧装甲
    public int enemy_armor_back;//后装甲
    public float enemy_damage_multiplier;//伤害乘子
    public int enemy_ammo_type;//弹种 0穿甲弹 1高爆弹 2破甲弹 3APCBC等后效弹

    public bool enemy_airstrike;//可否空袭
    public bool enemy_recon;//可否侦查
    public bool enemy_air_sup;//可否制空
    public bool enemy_artillery;//可否炮击
    public float enemy_skill_point;//技能点数
    public float enemy_skill_fullcharge;//技能所需点数
    public float enemy_moveTime;

    public bool enemy_withdrew;//是否下场
    public bool enemy_visible = false;//是否可见

    public int enemy_mag;//弹夹数量，如果不是弹夹炮那就是1发
    public int enemy_shell_speed;//弹速
}
