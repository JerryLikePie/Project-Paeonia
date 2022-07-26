using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Map2
{
	public class CameraCollisionControll : MonoBehaviour
	{
		public GameObject mouseManagerStub;
		private MouseManager2 mouseManager2;

		public GameObject mapManagerStub;
		private MapManager2 mapManager2;

		private string lastColliderName = "";

		private void Start()
		{
			mouseManager2 = mouseManagerStub.GetComponent<MouseManager2>();
			mapManager2 = mapManagerStub.GetComponent<MapManager2>();
		}

		// 将碰撞信息传递给 mouseManager
		private void OnTriggerEnter(Collider other)
		{
			/*
			// 防抖
			if (!lastColliderName.Equals(other.gameObject.name))
			{
				lastColliderName = other.gameObject.name;
				Debug.Log(lastColliderName);
				mouseManager2.cameraRewind(other.gameObject.name);
			}
			*/
		}
	}
}
