using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Assets.Scripts.BuffSystem.BuffeeImpl
{
	/// <summary>
	/// 代理模式实现
	/// 在 DollsProperty 和调用者中间的 Buff 代理层
	/// </summary>
	public sealed class DollsPropertyBuffed : DollsProperty, BuffUpdateListener
    {
		private DollsProperty dollsPropertyRaw;

		private List<BuffConstants.BuffId> buffIds = new List<BuffConstants.BuffId>();

		// 所有受到 attr-buff 影响的字段
		private Dictionary<BuffConstants.BuffId, FieldInfo> dictBuffedFields = new Dictionary<BuffConstants.BuffId, FieldInfo>();

		// 通过反射调用拷贝一份 DollsProperty
		public void getAndRegBuffedProperty(DollsProperty dollsProperty, BuffManager buffManager)
		{
			this.dollsPropertyRaw = dollsProperty;

			// 反射调用，拷贝被代理类所有成员
            var parentClass = typeof(DollsProperty);
			var fields = parentClass.GetFields();

            foreach(var field in fields)
			{
                field.SetValue(this, field.GetValue(dollsProperty));
			}

			// 反射检测注解，获取所有带 Buff 的属性
			dictBuffedFields = BuffedAttrAttribute.getBuffeFields(GetType());

            // 初始化带 Buff 标记的字段为原始值，避免默认值污染
            foreach (var kvp in dictBuffedFields)
            {
                try
                {
                    var originField = parentClass.GetField(kvp.Value.Name);
                    if (originField != null)
                    {
                        kvp.Value.SetValue(this, originField.GetValue(this));
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("init buffed field failed: " + kvp.Value.Name + " | " + ex);
                }
            }

			// 注册监听器
			buffManager.addListener(this);
		}

		public HashSet<BuffConstants.BuffId> interestBuffIds()
		{
			return new HashSet<BuffConstants.BuffId>(dictBuffedFields.Keys);
		}

		// buff 更新时重新计算相应属性的值
		public void onBuffUpdate(BuffManager buffManager, Buff buff)
		{
			FieldInfo field, myField;
			if (buff.buffType == BuffConstants.BuffType.BUFF_ATTR)
			{
				if (dictBuffedFields.TryGetValue(buff.buffId, out myField))
				{
					field = typeof(DollsProperty).GetField(myField.Name);

					object originVal = field.GetValue(dollsPropertyRaw); ;
					// 因为是反射获取的 object 型属性，没法注入到 T 中，只好使用反射方法调用
					var buffedVal = Utilities.invokeTypedMethod(
						buffManager,
						"takeEffects",
						new Type[] { myField.FieldType },
						originVal, buff.buffId);
					myField.SetValue(this, buffedVal);

					Debug.Log("buffed: " + originVal + "->" + buffedVal);
				}
			}
		}

        // 受 buff 管控的属性字段
        [NonSerialized]
        [BuffedAttr(BuffConstants.BuffType.BUFF_ATTR, BuffConstants.BuffId.BUFF_ATTR_DAMAGE_MULTIPLIER)]
        public new float dolls_damage_multiplier;

        [NonSerialized]
        [BuffedAttr(BuffConstants.BuffType.BUFF_ATTR, BuffConstants.BuffId.BUFF_ATTR_ACCURACY)]
        public new int dolls_accuracy;

        [NonSerialized]
        [BuffedAttr(BuffConstants.BuffType.BUFF_ATTR, BuffConstants.BuffId.BUFF_ATTR_ARMOR_FRONT)]
        public new float dolls_armor_front;

        [NonSerialized]
        [BuffedAttr(BuffConstants.BuffType.BUFF_ATTR, BuffConstants.BuffId.BUFF_ATTR_ARMOR_SIDE)]
        public new int dolls_armor_side;

        [NonSerialized]
        [BuffedAttr(BuffConstants.BuffType.BUFF_ATTR, BuffConstants.BuffId.BUFF_ATTR_ARMOR_BACK)]
        public new int dolls_armor_back;

		//public string dolls_name;//名称
		//public int dolls_ammount;//有几个。之后可以加入心智升级之类的设定，虽然还不知道咋表现出来
		//public int dolls_star;//星级
		//public int dolls_id;//id
		//public int dolls_type;//1陆军 2支援 3空军 4防空
		//public int dolls_range;//攻击距离w
		//public int dolls_view_range;//视野
		//public bool dolls_unlocked;//是否已解锁
		//public float dolls_max_hp;//最大生命值

		/* example code: 在本类中，需要 buff 什么属性就用new覆盖该属性，并加上这样两个 Annotation
			[NonSerialized]
			[BuffedAttr(BuffConstants.BuffType.BUFF_ATTR, BuffConstants.BuffId.BUFF_ATTR_ATK)]
			public new float dolls_sts_attack; //地对地攻击力
		*/

		//public float dolls_sts_attack; //地对地攻击力
		//public float dolls_ata_attack;//空对空攻击力
		//public float dolls_sta_attack;//地对空攻击力
		//public float dolls_ats_attack;//空对地攻击力
		//public float dolls_penetration;//穿深
		//public int dolls_accuracy;//命中
		//public int dolls_dodge;//闪避

		//public float dolls_reload;//装填
		//public float dolls_firerate;//如果不是弹夹炮，那么这个开火时间就等于装填时间

		//public float dolls_armor_front;//前装甲
		//public int dolls_armor_side;//侧装甲
		//public int dolls_armor_back;//后装甲

		//public float dolls_damage_multiplier;//伤害乘子
		//public int dolls_ammo_type;//弹种 0穿甲弹 1高爆弹 2破甲弹 3apcbc等后效弹 4航空炸弹 5尾翼稳定脱壳穿甲弹

		//public float dolls_skill_point;//技能点数
		//public float dolls_skill_fullcharge;//技能所需点数

		//public bool dolls_withdrew;//是否下场

		//public int dolls_mag;//弹夹数量，如果不是弹夹炮那就是1发
		//public int dolls_shell_speed;//弹速
		//public int dolls_mag2;//如果有副武器的话

		//float storeattack;
		//float storereload;
		//float storepen;
	}
}