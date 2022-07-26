using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Map2
{
	public class MouseManager2 : MonoBehaviour
	{
		// 获取 MapManger2 地址
		[Tooltip("MapManager2 挂载的对象")]
		public GameObject mapManagerStub;
		private MapManager2 mapManager;

		// 用于检测相机位置的碰撞箱集合(已弃用)
		public GameObject cameraBounds;

		// 相机自身碰撞箱(已弃用)
		public GameObject cameraCollider;

		float xOffset = 8.655f;//无痕：8.65f，有：9f
		float xStep = 17.31f;//无痕：17.31f，有：17.75f
		float zStep = 14.99f;//无痕：15f，有：15.35f

		// 相机
		// 从地图格到相机的方向向量
		private Vector3 invCameraDirection = new Vector3(-Mathf.Sqrt(3), 2, -3);

		// 相机初始位置
		// private Vector3 cameraOrigin;

		// 当前相机瞄准的 Tile
		private Vector2 cameraLookAt;

		/* 是否启用，仅在游戏过程中启用，其他如开始、暂停、结束时禁用拖动功能 */
		public bool dragEnabled;

		// Use this for initialization
		void Start()
		{
			mapManager = mapManagerStub.GetComponent<MapManager2>();
			dragEnabled = false;
		}

		// 鼠标拖动的flag
		bool isDragging = false;
		// 拖动起始位置
		Vector3 dragStart;
		Vector3 dragEnd;
		// 相机拖动、缩放参数
		private readonly float MouseZoomSpeed = 15.0f, TouchZoomSpeed = 0.08f;
		private readonly float ZoomMinBound = 25f, ZoomMaxBound = 60f;
		private readonly float
			MIN_Y = 40f, MAX_Y = 140f,
			MAX_X = 135f, MIN_X = -40f,
			MIN_Z = -80f, MAX_Z = 40f;

		// Update is called once per frame
		void Update()
		{
			if (dragEnabled)
			{
				cameraDrag();
				getCameraLookAt();
			}
		}

		public void enableDrag()
		{
			this.dragEnabled = true;
		}

		public void disableDrag()
		{
			this.dragEnabled = false;
		}

		public void setCameraOrigin(GameObject tileLookAt)
		{
			/*
			this.cameraOrigin = posLookAt + 20 * invCameraDirection;
			Camera.main.transform.position = this.cameraOrigin;
			cameraBounds.transform.position = this.cameraOrigin;
			cameraCollider.transform.position = this.cameraOrigin;
			*/
			// 垂直方向上有一个固定偏移量
			// 然后向 lookat 反方向移动。因为是正交投影，因此不改变视野
			Camera.main.transform.position = tileLookAt.transform.position + (Vector3.up * 0.6f) + (invCameraDirection * 100f);
			cameraLookAt = Utilities.tileNameToPos(tileLookAt.transform.name);
		}

		public void getCameraLookAt()
		{
			Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
			Ray ray = Camera.main.ScreenPointToRay(screenCenter);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 1000.0f))
			{
				//绘制线，在Scene视图中可见
				Debug.DrawLine(ray.origin, hit.point, Color.green);
				//输出射线探测到的物体的名称
				if (hit.transform.name.Equals("Hex") && hit.transform.parent != null)
				{
					// Debug.Log("射线探测到的物体：" + hit.transform.parent.name);
					string tileName = hit.transform.parent.name;
					Vector2 pos = Utilities.tileNameToPos(hit.transform.parent.name);
					int dRow = (int)(pos.x - cameraLookAt.x);
					int dCol = (int)(pos.y - cameraLookAt.y);
					cameraLookAt.x = pos.x;
					cameraLookAt.y = pos.y;
					// 调整显示视野
					if (dRow != 0 || dCol != 0)
					{
						mapManager.moveHorizon(dRow, dCol);
					}
				}
			}
		}

		private void cameraDrag()
		{
			// 触屏单指或非触屏
			if ((Input.touchSupported && Input.touchCount == 1) 
				|| !Input.touchSupported)
			{
				// 上升沿触发记录起始坐标
				if (!isDragging && Input.GetMouseButton(0))
				{
					dragStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				}
				isDragging = Input.GetMouseButton(0);
				// 持续记录 dragEnd
				dragEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				if (isDragging)
				{
					Vector3 diff = dragEnd - dragStart;
					Camera.main.transform.position -= diff;
					/*
					// 将相机碰撞箱映射到 cameraOrigin 平面
					Vector3 cpos = Camera.main.transform.position;
					float fy = cameraOrigin.y - cpos.y;
					Vector3 tpos = new Vector3();
					tpos.x = fy * (invCameraDirection.x / invCameraDirection.y);
					tpos.y = fy;
					tpos.z = fy * (invCameraDirection.z / invCameraDirection.y);
					cameraCollider.transform.position = cpos + tpos;
					*/
					// 因为 Raycast 距离有限，因此需要限制相机高度
					float CH = Camera.main.transform.position.y;
					if (CH < 5f)
					{
						Camera.main.transform.position += invCameraDirection * 100f;
					}
					else if (CH > 300f)
					{
						Camera.main.transform.position -= invCameraDirection * 100f;
					}
				}
			}
		}

		/*
		// 相机触碰边界修改碰撞盒位置
		public void cameraRewind(string where)
		{
			Vector3 newPos = cameraOrigin;
			switch(where)
			{
				case "left":
					newPos.x -= xStep;
					break;
				case "right":
					newPos.x += xStep;
					break;
				case "leftTop":
					newPos.x -= xOffset;
					newPos.z += zStep;
					break;
				case "rightTop":
					newPos.x += xOffset;
					newPos.z += zStep;
					break;
				case "leftBottom":
					newPos.x -= xOffset;
					newPos.z -= zStep;
					break;
				case "rightBottom":
					newPos.x += xOffset;
					newPos.z -= zStep;
					break;
			}
			cameraBounds.transform.position = newPos;
			cameraOrigin = newPos;
		}
		*/
	}
}