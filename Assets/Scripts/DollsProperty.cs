using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DollsProperty : MonoBehaviour
{
    public string dolls_name;//名称
    public int dolls_ammount;//有几个。之后可以加入心智升级之类的设定，虽然还不知道咋表现出来
    public int dolls_star;//星级
    public int dolls_id;//ID
    public int dolls_type;//1陆军 2支援 3空军 4防空
    public int dolls_range;//攻击距离
    public int dolls_view_range;//视野
    public bool dolls_unlocked;//是否已解锁
    public float dolls_max_hp;//最大生命值
    public float dolls_sts_attack;//地对地攻击力
    public float dolls_ata_attack;//空对空攻击力
    public float dolls_sta_attack;//地对空攻击力
    public float dolls_ats_attack;//空对地攻击力
    public float dolls_penetration;//穿深
    public int dolls_accuracy;//命中
    public int dolls_dodge;//闪避

    public float dolls_reload;//装填
    public float dolls_firerate;//如果不是弹夹炮，那么这个开火时间就等于装填时间

    public float dolls_armor_front;//前装甲
    public int dolls_armor_side;//侧装甲
    public int dolls_armor_back;//后装甲

    public float dolls_damage_multiplier;//伤害乘子
    public int dolls_ammo_type;//弹种 0穿甲弹 1高爆弹 2破甲弹 3APCBC等后效弹 4航空炸弹 5尾翼稳定脱壳穿甲弹

    public float dolls_skill_point;//技能点数
    public float dolls_skill_fullcharge;//技能所需点数

    public bool dolls_withdrew;//是否下场

    public int dolls_mag;//弹夹数量，如果不是弹夹炮那就是1发
    public int dolls_shell_speed;//弹速
    public int dolls_mag2;//如果有副武器的话

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
