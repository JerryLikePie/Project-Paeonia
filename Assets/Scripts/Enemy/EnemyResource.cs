using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utilities;

public class EnemyResource : IEnemyBehavior
{
    private DollsCombat doll;
    // 矿物
    // 矿物类敌人不攻击，不移动，不被当做攻击目标，但是可以被轰炸类手动技能影响。当被攻击时，获得95%的减伤。
    // 当我方单位站在矿物类单位相邻格子时，矿物类敌人开始自我分解（每秒受到1点伤害，生命值20）
    // 矿物类单位死亡时，掉落一定量的矿物，进入距离最近的dolls的背包，同时地图增加等同于矿物类敌人生命值的活性值
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
                    // 如果是空军的话无视
                    continue;
                }
                if (FindDistance(transform.gameObject, doll.gameObject) <= 17.5f * 1.2f)
                {
                    if (doll.gameObject.activeSelf)
                    {
                        //开始流血
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
