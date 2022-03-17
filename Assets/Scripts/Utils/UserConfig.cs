using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Utils
{
	public class UserConfig : MonoBehaviour
	{

		private const string configFileName = "user.dat";

		private WWW www;

		// Use this for initialization
		void Start()
		{
			www = new WWW(Application.streamingAssetsPath + "/" + configFileName);
		}

		// Update is called once per frame
		void Update()
		{

		}
	}
}