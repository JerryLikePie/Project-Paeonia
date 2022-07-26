using Assets.Scripts.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Map2
{
    public class MapManager2 : MonoBehaviour
    {
        public GameObject mouseManagerStub;
        private MouseManager2 mouseManager;

        float xOffset = 8.655f;//无痕：8.65f，有：9f
        float xStep = 17.31f;//无痕：17.31f，有：17.75f
        float zStep = 14.99f;//无痕：15f，有：15.35f

        // 地图块类型
        public List<TileType> tileTypes;
        // 
        // 
        string currentStageName;
        MapInfo mapInfo;

        // 整张地图
        List<List<GameObject>> mapTiles;

        // 当前地图视野，设定为 5x5 测试
        [Header("可见地图范围")]
        [SerializeField, FieldName("相机初始位置")]
        Vector2 horizonCenter = new Vector2(3, 3);
        [SerializeField, FieldName("左下角距中心偏移")]
        Vector2 horizonTL = new Vector2(-2, -2);
        [SerializeField, FieldName("右上角距中心偏移")]
        Vector2 horizonBR = new Vector2(2, 2);

        // 地图显隐动画
        private List<TileAnimation> animatingTiles;
        // 显隐动画开始、结束位置
        private readonly float TILE_ANIM_END_POS = 3f;
        // 显隐动画播放速度
        private readonly float TILE_ANIM_SPEED = 8f;

        // Use this for initialization
        void Start()
		{
            mapTiles = new List<List<GameObject>>();
            mouseManager = mouseManagerStub.GetComponent<MouseManager2>();
            animatingTiles = new List<TileAnimation>();
        }

        private List<TileAnimation> removeList = new List<TileAnimation>();  // 待移除的元素

		// Update is called once per frame
		void Update()
		{
            if (removeList.Count != 0)
			{
                removeList.Clear();
			}
            // 处理地图格渐入渐出动画
            foreach (TileAnimation anim in animatingTiles)
			{
                // 渐入
                if (anim.target == TileAnimation.T_DOWN)
				{
                    anim.obj.transform.position += Vector3.down * TILE_ANIM_SPEED * Time.deltaTime; // deltaTime 是频率的倒数
                    if (anim.obj.transform.position.y <= 0f)
					{
                        removeList.Add(anim);
					}
				}
                // 渐隐
                else if (anim.target == TileAnimation.T_UP)
				{
                    anim.obj.transform.position += Vector3.up * TILE_ANIM_SPEED * Time.deltaTime;
                    if (anim.obj.transform.position.y >= TILE_ANIM_END_POS) // 3f 为动画结束位置
					{
                        anim.obj.SetActive(false);
                        removeList.Add(anim);
					}
				}
            }
            // 批量删除
            foreach (TileAnimation anim in removeList)
            {
                animatingTiles.Remove(anim);
            }
        }

        public void LoadMap()
        {
            currentStageName = "Fuckin_Big_Map"; // fixme debug
            TextAsset mapInJson = Resources.Load(currentStageName + "_json") as TextAsset;
            mapInfo = JsonUtility.FromJson<MapInfo>(mapInJson.text);

            int M_ROWS = mapInfo.mapTiles.Length;
            int M_COLS = mapInfo.mapTiles[0].Length;

            // 加载所有地图块
            // 默认禁用状态
            for (int row = 0; row < M_ROWS; row++)
            {
                List<GameObject> rowTiles = new List<GameObject>();
                for (int col = 0; col < M_COLS; col++)
                {
                    int tileCode = mapInfo.mapTiles[row][col] - '0';

                    if (mapInfo.mapTiles[row][col] == 'W')
                    {
                        tileCode = 7;
                    }
                    if (mapInfo.mapTiles[row][col] == 'R' || mapInfo.mapTiles[row][col] == 'B')
                    {
                        tileCode = 0;
                    }
                    GameObject newTile = tileTypes[tileCode].tilePrefabType;
                    if (col % 2 == 0)
                    {
                        newTile = Instantiate(newTile, new Vector3(row * xStep, 0, col * zStep), Quaternion.identity);
                    }
                    else
                    {
                        newTile = Instantiate(newTile, new Vector3((row * xStep) - xOffset, 0, col * zStep), Quaternion.identity);
                    }
                    // 先行号后列号
                    newTile.name = Utilities.tilePosToName(row, col);
                    newTile.transform.parent = this.gameObject.transform;
                    newTile.GetComponent<Hex>().X = row;
                    newTile.GetComponent<Hex>().Z = col;
                    newTile.GetComponent<Hex>().render = true;
                    newTile.SetActive(false);
                    rowTiles.Add(newTile);
                }
                mapTiles.Add(rowTiles);
            }

            GameObject centerTile = mapTiles[(int)horizonCenter.x][(int)horizonCenter.y];
            Vector2 TopLeft = horizonCenter + horizonTL;
            Vector2 BottomRight = horizonCenter + horizonBR;
			for (int row = Math.Max(0, (int)TopLeft.x); row <= Math.Min(M_ROWS - 1, (int)BottomRight.x); row++)
			{
				for (int col = Math.Max(0, (int)TopLeft.y); col <= Math.Min(M_COLS - 1, (int)BottomRight.y); col++)
                {
                    mapTiles[row][col].SetActive(true);
                }
			}
            // 设置相机看向的 tile
            mouseManager.setCameraOrigin(centerTile);
            
        }
        
        // 动态修改地图区域
        public void moveHorizon(int dRow, int dCol)
        {
            int M_ROWS = mapInfo.mapTiles.Length;
            int M_COLS = mapInfo.mapTiles[0].Length;
            // 前行号后列号
            Vector2 dPos = new Vector2(dRow, dCol);
            Vector2 oldTopLeft = horizonCenter + horizonTL;
            Vector2 oldBottomRight = horizonCenter + horizonBR;
            Vector2 newTopLeft = oldTopLeft + dPos;
            Vector2 newBottomRight = oldBottomRight + dPos;
            HashSet<GameObject> tilesToShow = new HashSet<GameObject>();
            HashSet<GameObject> tilesToHide = new HashSet<GameObject>();
            // 修改视野
            horizonCenter += dPos;
            // 增加新行
            if (dRow > 0)
			{
                for (int row = Math.Max(0, (int)oldBottomRight.x + 1); row <= Math.Min(M_ROWS - 1, newBottomRight.x); row++)
				{
                    for (int col = Math.Max(0, (int)newTopLeft.y); col <= Math.Min(M_COLS - 1, newBottomRight.y); col++)
					{
                        tilesToShow.Add(mapTiles[row][col]);
					}
				}
                for (int row = Math.Max(0, (int)oldTopLeft.x); row < Math.Min(M_ROWS, newTopLeft.x); row++)
				{
                    for (int col = Math.Max(0, (int)oldTopLeft.y); col <= Math.Max(M_COLS - 1, oldBottomRight.y); col++)
					{
                        tilesToHide.Add(mapTiles[row][col]);
					}
				}
			}
            else if (dRow < 0)
            {
                for (int row = Math.Max(0, (int)newTopLeft.x); row < Math.Min(M_ROWS, oldTopLeft.x); row++)
                {
                    for (int col = Math.Max(0, (int)newTopLeft.y); col <= Math.Min(M_COLS - 1, newBottomRight.y); col++)
                    {
                        tilesToShow.Add(mapTiles[row][col]);
                    }
                }
                for (int row = Math.Max(0, (int)newBottomRight.x + 1); row <= Math.Min(M_ROWS - 1, oldBottomRight.x); row++)
                {
                    for (int col = Math.Max(0, (int)oldTopLeft.y); col <= Math.Min(M_COLS - 1, oldBottomRight.y); col++)
                    {
                        tilesToHide.Add(mapTiles[row][col]);
                    }
                }
            }
            // 增加新列
            if (dCol > 0)
			{
                for (int col = Math.Max(0, (int)oldBottomRight.y + 1); col <= Math.Min(M_COLS - 1, newBottomRight.y); col++)
				{
                    for (int row = Math.Max(0, (int)newTopLeft.x); row <= Math.Min(M_ROWS - 1, newBottomRight.x); row++)
					{
                        tilesToShow.Add(mapTiles[row][col]);
					}
                }
                for (int col = Math.Max(0, (int)oldTopLeft.y); col < Math.Min(M_COLS, newTopLeft.y); col++)
                {
                    for (int row = Math.Max(0, (int)oldTopLeft.x); row <= Math.Min(M_ROWS - 1, oldBottomRight.x); row++)
                    {
                        tilesToHide.Add(mapTiles[row][col]);
                    }
                }
            }
            else if (dCol < 0)
			{
                for (int col = Math.Max(0, (int)newTopLeft.y); col < Math.Min(M_COLS, oldTopLeft.y); col++)
				{
                    for (int row = Math.Max(0, (int)newTopLeft.x); row <= Math.Min(M_ROWS - 1, newBottomRight.x); row++)
					{
                        tilesToShow.Add(mapTiles[row][col]);
					}
                }
                for (int col = Math.Max(0, (int)newBottomRight.y + 1); col <= Math.Min(M_COLS - 1, oldBottomRight.y); col++)
                {
                    for (int row = Math.Max(0, (int)oldTopLeft.x); row <= Math.Min(M_ROWS - 1, oldBottomRight.x); row++)
                    {
                        tilesToHide.Add(mapTiles[row][col]);
                    }
                }
            }
            showTilesAnimated(tilesToShow);
            hideTilesAnimated(tilesToHide);
            /*
            Vector3 newPos = cameraCollider.transform.position;
            Vector3 remPos = cameraCollider.transform.position - cameraOrigin;
            switch (where)
            {
                case "left":
                    newPos.x += xStep;
                    break;
                case "right":
                    newPos.x -= xStep;
                    break;
                case "leftTop":
                    newPos.x += xOffset;
                    newPos.z -= zStep;
                    break;
                case "rightTop":
                    newPos.x -= xOffset;
                    newPos.z -= zStep;
                    break;
                case "leftBottom":
                    newPos.x += xOffset;
                    newPos.z += zStep;
                    break;
                case "rightBottom":
                    newPos.x -= xOffset;
                    newPos.z += zStep;
                    break;
            }
            */
        }

        // 地图格渐入
        private void showTilesAnimated(IEnumerable<GameObject> tiles)
		{
            TileAnimation anim;
            foreach(GameObject tile in tiles)
			{
                tile.SetActive(true);
                if ((anim = animatingTiles.Find(o => o.obj == tile)) != null)
                {
                    anim.target = TileAnimation.T_DOWN;
                }
                else
                {
                    tile.transform.position += Vector3.up * TILE_ANIM_END_POS;
                    animatingTiles.Add(new TileAnimation { obj = tile, target = TileAnimation.T_DOWN });
                }
			}
		}

        // 地图格渐隐
        private void hideTilesAnimated(IEnumerable<GameObject> tiles)
        {
            TileAnimation anim;
            foreach (GameObject tile in tiles)
            {
                if ((anim = animatingTiles.Find(o => o.obj == tile)) != null)
                {
                    anim.target = TileAnimation.T_UP;
                }
                else
                {
                    animatingTiles.Add(new TileAnimation { obj = tile, target = TileAnimation.T_UP });
                }
            }
        }

    }

    class TileAnimation
	{
        public static int T_DOWN = 0;
        public static int T_UP = 1;
        public GameObject obj;
        public int target; // 0 - down to earth; 1 - up to sky
	}

    [System.Serializable]
    class EnemySpawnPoint
    {
        public int spawnType;
        public string spawnTile;
        public int spawnTime;
        public string[] nextTile;
        public int[] waitTime;
    }
    class MapInfo
    {
        public string[] mapTiles;
        public EnemySpawnPoint[] enemySpawnPoints;
        public int timeLimit;
        public int dropID;
        public int dropAmount;
        public int dropRate;

        public override string ToString()
        {
            string str = "";
            foreach (EnemySpawnPoint e in enemySpawnPoints)
            {
                str += "spawnTile: " + e.spawnTile + "\n";
                foreach (string s in e.nextTile)
                {
                    str += "  nextTile: " + s + "\n";
                }
            }
            return str;
        }
    }
}