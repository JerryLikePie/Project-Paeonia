using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Utils
{
	// 用于在安卓本地持久化用户配置
	// fixme Unity自带的JSON也太拉跨了，现在text写文件没问题，但是没法dict转json text，明天再修
	public class LocalConfig : MonoBehaviour
	{
		// 配置文件的文件名
		public const string USER_CONFIG_FNAME = "user_config.json";

		// 配置文件的路径，在 Start 中动态设置
		private string configPath = "";

		public Dictionary<string, object> configs;

		//
		void Start()
		{
			RuntimePlatform platform = Application.platform;

			if (platform == RuntimePlatform.Android)
			{
				configPath = Path.Combine(Application.persistentDataPath, USER_CONFIG_FNAME);
			}
			else if (platform == RuntimePlatform.WindowsEditor || platform == RuntimePlatform.WindowsPlayer)
			{
				configPath = Path.GetFullPath(Path.Combine(Application.persistentDataPath, USER_CONFIG_FNAME));
			}

			configs = new Dictionary<string, object>();

			setConfig("Hello", "World");
			setConfig("Gloria", "Cool");
		}

		public void setConfig(string configKey, object value)
		{
			configs[configKey] = value;

			string configJson = Utilities.SerializeDictionaryToJson(configs);

			if (!File.Exists(configPath))
			{
				File.CreateText(configPath).Dispose();
			}
			else
			{
				Debug.Log("[LocalConfig] " + configPath);
			}
			using (StreamWriter streamWriter = new StreamWriter(this.configPath))
			{
				streamWriter.Write(configJson);
				streamWriter.Flush();
			}
		}

	}
}