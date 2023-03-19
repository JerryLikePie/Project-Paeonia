using System.Collections;
using UnityEngine;

namespace Assets.Scripts.SquardOverviewManager
{
    /// <summary>
    /// 用于保存 doll 的详细信息供面板显示
    /// </summary>
    public class DollDetailInfo : MonoBehaviour
    {
        // 图鉴界面显示的角色立绘
        public Sprite dollOverviewImage;
        // 详情界面显示的角色立绘
        public Sprite dollDetailImage;
        // 详情界面显示的角色SD
        public Sprite SDDolls;
        // 角色数值和本体
        public DollsProperty dollsDetail;
        // 武器和技能的图标和描述
        public Sprite SDWeapon;
        [TextArea(3, 5)] public string weapon_description;
        public Sprite[] SDSkill;
        [TextArea(3, 5)] public string[] skill_description;
    }
}