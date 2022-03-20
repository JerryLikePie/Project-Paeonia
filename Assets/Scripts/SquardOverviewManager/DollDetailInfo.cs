using System.Collections;
using UnityEngine;

namespace Assets.Scripts.SquardOverviewManager
{
    /// <summary>
    /// 用于保存 doll 的详细信息供面板显示
    /// </summary>
    public class DollDetailInfo : MonoBehaviour
    {
        // 角色名称
        public string dollName;
        // 图鉴界面显示的角色立绘
        public Sprite dollOverviewImage;
        // 详情界面显示的角色立绘
        public Sprite dollDetailImage;
    }
}