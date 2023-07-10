using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Assets.Scripts.BuffSystem.BuffedImpl
{
	/// <summary>
	/// 代理模式实现
	/// 在 DollsProperty 和调用者中间的 Buff 代理层
	/// </summary>
	public sealed class DollsPropertyBuffed : DollsProperty, BuffUpdateListener
    {
		private DollsProperty dollsPropertyRaw;

		private List<BuffConstants.BuffId> buffIds = new List<BuffConstants.BuffId>();

		// todo (maybe) 一个 buff 影响多个 field
		private Dictionary<BuffConstants.BuffId, FieldInfo> valBuffedFields = new Dictionary<BuffConstants.BuffId, FieldInfo>();

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
			// 实现 listener 再调用 takeEffects 总感觉似乎点多余，但也更灵活了。未来也可能会考虑全部放到 buffManager 里面实现
			valBuffedFields = BuffManager.getBuffedFields(GetType(), BuffConstants.BuffType.BUFF_ATTR);

			// 注册监听器
			buffManager.addListener(this);
		}

		public HashSet<BuffConstants.BuffId> interestBuffIds()
		{
			return new HashSet<BuffConstants.BuffId>(valBuffedFields.Keys);
		}

		public void onBuffUpdate(BuffManager buffManager, Buff buff)
		{
			FieldInfo field, myField;
			if (buff.buffType == BuffConstants.BuffType.BUFF_ATTR)
			{
				if (valBuffedFields.TryGetValue(buff.buffId, out myField))
				{
					field = typeof(DollsProperty).GetField(myField.Name);

					object originVal = field.GetValue(dollsPropertyRaw); ;

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

		//public string dolls_name;//名称
		//public int dolls_ammount;//有几个。之后可以加入心智升级之类的设定，虽然还不知道咋表现出来
		//public int dolls_star;//星级
		//public int dolls_id;//id
		//public int dolls_type;//1陆军 2支援 3空军 4防空
		//public int dolls_range;//攻击距离w
		//public int dolls_view_range;//视野
		//public bool dolls_unlocked;//是否已解锁
		//public float dolls_max_hp;//最大生命值
		// example: 需要 buff 什么属性就加上这样两个 Annotation
		[NonSerialized]
		[BuffedAttr(BuffConstants.BuffType.BUFF_ATTR, BuffConstants.BuffId.BUFF_ATTR_ATK)]
		public new float dolls_sts_attack; //地对地攻击力
		//public float dolls_ata_attack;//空对空攻击力
		//public float dolls_sta_attack;//地对空攻击力
		//public float dolls_ats_attack;//空对地攻击力
		//public float dolls_penetration;//穿深
		//public int dolls_accuracy;//命中
		//public int dolls_dodge;//闪避

		//// todo example code
		//public buffedattr<int> dolls_atk = new buffedattr<int>(buffconstants.buffid.buff_val_atk);

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