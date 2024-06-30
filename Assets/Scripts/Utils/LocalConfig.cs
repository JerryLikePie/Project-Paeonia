using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;


namespace Assets.Scripts.Utils
{
	///
	/// 用于在安卓本地持久化用户配置
	///
	/// APIs:
	///   setConfig("someFlag", true);					// 将 configs.someFlag 的值设置为 true
	///   setConfig("field_not_exist", "wrong usage");	// 如果设置了不存在的字段，将会抛出错误
	///
	/// 添加和使用新配置项的流程：
	///   1. 在 Configs 类中添加 public 成员变量
	///      例如： public string a;
	///   2. 在需要修改设置的地方通过本类的实例调用 setConfig 方法来设置它的值
	///      例如：localConfig.setConfig("a", "json-utility sucks");
	///   
	public class LocalConfig : MonoBehaviour
	{
		// 配置文件的文件名
		public const string USER_CONFIG_FNAME = "user_config.json";

		// 配置文件的路径，在 Start 中动态设置
		private string configPath = "";

		private Configs configs;

		public Configs GetConfigs()
		{
			return configs;
		}

		// 配置类
		// 
		// 本来想用 key-value 来做，但是 JSONUtility 不支持 Dictionary 的序列化，所以退而求其次选择用 class
		// 禁止改 class 为 struct，会导致无法序列化
		public class Configs
		{
			/// Example
			/// public bool someFlag;
		}

		// 初始化
		void Start()
		{
			RuntimePlatform platform = Application.platform;

			if (platform == RuntimePlatform.Android)
			{
				configPath = Path.Combine(Application.persistentDataPath, USER_CONFIG_FNAME);
			}
			else if (platform == RuntimePlatform.WindowsEditor || platform == RuntimePlatform.WindowsPlayer)
			{
				configPath = Path.Combine(Application.persistentDataPath, USER_CONFIG_FNAME);
			}

			configs = new Configs();
		}

		/// <summary>
		/// 修改设置。
		/// 修改的内容会当场自动保存
		/// </summary>
		/// <typeparam name="T">自动推断，一般情况不用管。部分情况需要手动指定，如value: null时</typeparam>
		/// <param name="key">要修改哪个字段的值。字段必须存在，否则会报错</param>
		/// <param name="value">要把指定字段设置为什么值</param>
		public void setConfig<T>(string key, T value)
		{
			// 类反射
			Type configType = typeof(Configs);

			// 获取对应属性(Property)的反射
			PropertyInfo propertyInfo = configType.GetProperty(key);

			if (propertyInfo != null && propertyInfo.CanWrite)
			{
				propertyInfo.SetValue(configs, value);
			}
			else
			{
				// 如果不是属性，那就按照字段(Field)的方式访问
				FieldInfo fieldInfo = configType.GetField(key);

				if (fieldInfo != null)
				{
					fieldInfo.SetValue(configs, value);
				}
				else
				{
					throw new ArgumentException($"指定的属性\"{key}\"不存在或不支持修改");
				}
			}

			// 保存到本地
			saveConfigs();
		}

		/// <summary>
		/// 保存设置。
		/// 目前不必手动调用，每次 setConfig 会自动调用 saveConfigs。除非直接设置了 Configs 中的值
		/// </summary>
		public void saveConfigs()
		{
			string configJson = JsonUtility.ToJson(configs);

			if (!File.Exists(configPath))
			{
				File.CreateText(configPath).Dispose();
			}
			using (StreamWriter streamWriter = new StreamWriter(this.configPath))
			{
				streamWriter.Write(configJson);
				streamWriter.Flush();
			}
			Debug.Log($"[LocalConfig] local configs saved at {configPath}");
		}

	}
}