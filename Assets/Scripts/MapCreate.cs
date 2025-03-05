using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Utilities;

// todo 感觉应该改名叫 LevelCreator 之类的（
public class MapCreate : MonoBehaviour
{
    //我好tm渴啊，真的，喝牛奶是真的不解渴
    //还是得喝水，但是水又没味道
    public List<TileType> tileTypes;
    public Unit[] spawnSquad = null;
    public USerializableDictionary<string, Unit> spawnSquardA;
    public EnemyProperty[] spawnEnemy = null;
    public GameObject[] SkillSlot;
    public GameObject[] Skills;
    public GameObject ObjectivePoint;
    public GameObject HomePoint;
    float zOffset = 8.655f;//无痕：8.65f，有：9f
    float xOffset = 17.31f;//无痕：17.31f，有：17.75f
    float hangOffset = 14.99f;//无痕：15f，有：15.35f
    public GameObject enemyList;
    public GameObject unitList;
    int homeHang = 0;
    int homeLie = 0;
    public float timeLimit;

    [HideInInspector] public Hex RedPoint, BluePoint;
    int maxX, maxZ;
    public SquadSlot[] slots;
    int dropID, dropAmount, dropRate;
    [HideInInspector] public string mapToLoad;

    // 游戏核心组件
    public GameCore gameCore;

	[System.Serializable]
    class EnemySpawnPoint
    {
        public int spawnType;
        public string spawnTile;
        public int spawnTime;
        public string[] nextTile;
        public int[] waitTime;
    }
    // 可以持续生成敌人的生成点
    class EnemyContinuousSpawnPoint
	{
        public string baseSpawnTile;
        public Queue<int> spawnPool;
	}
    class MapInfo
    {
        public string[] mapTiles;
        public EnemySpawnPoint[] enemySpawnPoints;
        public EnemyContinuousSpawnPoint[] enemyConinuousSpawnPoints;
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
    void Start()
    {
        mapToLoad = PlayerPrefs.GetString("Stage_You_Should_Load", "Map_1-1");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool IsTutorial()
    {
        return mapToLoad.StartsWith("TR");
    }

    public bool IsOperation()
	{
        return mapToLoad.StartsWith("OP");
	}

    public void SpawnGameWithPreset()
    {
        // 如果是教程模式则使用预设
        // 这里要改一改，改成不同教程给不同预设
        FillUnitSlotWithPreset(BluePoint, 1);
        SpawnGame();
	}

    public void SpawnGame(bool generateRandomMap = false)
    {
        MapInfo mapInfo;

        //首先，生成地图本体
        Debug.Log("map to load is: " + mapToLoad);
        if (!generateRandomMap)
        {
            // 加载地图文件
            TextAsset textToMapJson = (TextAsset)Resources.Load(mapToLoad + "_json");
            mapInfo = JsonUtility.FromJson<MapInfo>(textToMapJson.text);
        }
        else
		{
            // 生成随机地图
            mapInfo = GetGeneratedRandomMapInfo();
		}
        Debug.Log("mapInfo:\n" + mapInfo);
        this.timeLimit = mapInfo.timeLimit;
        this.maxZ = mapInfo.mapTiles.Length;
        this.maxX = mapInfo.mapTiles[0].Length;
        this.dropID = mapInfo.dropID;
        this.dropAmount = mapInfo.dropAmount;
        this.dropRate = mapInfo.dropRate;
        //Debug.Log(mapInfo.dropID);
        //Debug.Log(mapInfo.dropAmount);
        //Debug.Log(mapInfo.dropRate);

        for (int i = 0; i < maxZ; i++)
        {
            for (int j = 0; j < maxX; j++)
            {
                GameObject thisTile = tileTypes[7].tilePrefabType; //先默认为都生成虚空
                
                if (mapInfo.mapTiles[i][j] == '0')//如果文件说这里是水
                {
                    thisTile = tileTypes[0].tilePrefabType;
                }
                else if (mapInfo.mapTiles[i][j] == '1')//如果文件说这里是草地
                {
                    thisTile = tileTypes[1].tilePrefabType;
                }
                else if (mapInfo.mapTiles[i][j] == '2')//如果文件说这里是沙地
                {
                    thisTile = tileTypes[2].tilePrefabType;
                }
                else if (mapInfo.mapTiles[i][j] == '3')//如果文件说这里是沼泽
                {
                    thisTile = tileTypes[3].tilePrefabType;
                }
                else if (mapInfo.mapTiles[i][j] == '4')//如果文件说这里是树林
                {
                    thisTile = tileTypes[4].tilePrefabType;
                }
                else if (mapInfo.mapTiles[i][j] == '5')//如果文件说这里是高地
                {
                    thisTile = tileTypes[5].tilePrefabType;
                }
                else if (mapInfo.mapTiles[i][j] == '6')//如果文件说这里是山地
                {
                    thisTile = tileTypes[6].tilePrefabType;
                }
                else if (mapInfo.mapTiles[i][j] == 'B')//如果文件说这里是我方的家
                {
                    thisTile = tileTypes[8].tilePrefabType;
                    this.homeHang = i;
                    this.homeLie = j;
                }
                else if (mapInfo.mapTiles[i][j] == 'R')//如果文件说这里是敌方的家
                {
                    thisTile = tileTypes[9].tilePrefabType;
                }
                //虽然可以直接tileTypes[mapFromText[i][j]]但是因为要设定可到达还有移动点数，干脆全列出来算了
                //有什么办法可以优化吗？

                if (i % 2 == 0)
                {
                    thisTile = Instantiate(thisTile, new Vector3(j * xOffset, 0, i * hangOffset), Quaternion.identity);
                    try
                    {
                        thisTile.GetComponent<Hex>().fogOfWarDarken.GetComponent<Renderer>().material.mainTextureOffset
                        = Vector2.up * (Random.value * 2f - 0.5f) + Vector2.right * (Random.value * 2f - 0.5f);
                    } catch{}
                    //thisTile.transform.Rotate(Vector3.up * 60f * Mathf.Floor(Random.value * 0.5f));
                }
                else
                {
                    thisTile = Instantiate(thisTile, new Vector3((j * xOffset) - zOffset, 0, i * hangOffset), Quaternion.identity);
                    try
                    {
                        thisTile.GetComponent<Hex>().fogOfWarDarken.GetComponent<Renderer>().material.mainTextureOffset
                        = Vector2.up * (Random.value * 2f - 0.5f) + Vector2.right * (Random.value * 2f - 0.5f);
                    } catch { }
                    //thisTile.transform.Rotate(Vector3.up * 60f * Mathf.Floor(Random.value * 0.5f));
                }
                //给这个地图块命名
                thisTile.name = "Map" + i + "_" + j;
                // todo 性能测试: 禁用地图块的动画
    //            foreach(var anim in thisTile.GetComponentsInChildren<Animator>())
				//{
    //                anim.enabled = false;
				//}
                try
                {
                    //但命名只是给我们看的，程序也要知道
                    thisTile.GetComponent<Hex>().X = i;
                    thisTile.GetComponent<Hex>().Z = j;
                    thisTile.GetComponent<Hex>().render = false;
                    if (mapInfo.mapTiles[i][j] == 'R')
                    {
                        this.RedPoint = thisTile.GetComponent<Hex>();
                        this.RedPoint.endGame = true;//如果是红点的话，相当于踩到这个点就游戏结束
                        this.ObjectivePoint = thisTile;
                    }
                    if (mapInfo.mapTiles[i][j] == 'B')
                    {
                        this.BluePoint = thisTile.GetComponent<Hex>();
                        this.BluePoint.endGame = true;//如果是红点的话，相当于踩到这个点就游戏结束
                        this.HomePoint = thisTile;
                        Camera.main.transform.position = HomePoint.transform.position + 78 * Vector3.up + 40 * Vector3.left + -78 * Vector3.forward;
                    }
                }
                catch
                { continue; }
                //把所有地图块都装入一个地图文件夹里面
                thisTile.transform.parent = this.gameObject.transform;
            }
        }
        InitialMapVision(homeHang, homeLie);
        SpawnTheUnits(homeHang, homeLie);
        SpawnTheEnemy(mapInfo);
        //Debug.Log("宽和长分别为" + maxX + "和" + maxZ);
    }
    
    private MapInfo GetGeneratedRandomMapInfo()
	{
        MapInfo mapInfo = new MapInfo();

        // todo 地图生成参数设置
        MapGenerator mapGen = new MapGenerator();
        mapGen.setGenre(MapGenerator.MapGenre.Grassland).enableRiver(false);
        // 生成地图
        mapInfo.mapTiles = mapGen.generate(50, 50);

        // todo 测试持续生成点
        EnemyContinuousSpawnPoint continuousSpawnPoint = new EnemyContinuousSpawnPoint();
        continuousSpawnPoint.baseSpawnTile = "Map5_18";
        continuousSpawnPoint.spawnPool = new Queue<int>();
        continuousSpawnPoint.spawnPool.Enqueue(1);

        // todo 检测随机生成的地图的可达性

        mapInfo.enemySpawnPoints = new EnemySpawnPoint[0];
        mapInfo.enemyConinuousSpawnPoints = new EnemyContinuousSpawnPoint[1];
        mapInfo.timeLimit = 1200;
        mapInfo.dropID = 1;
        mapInfo.dropAmount = 15;
        mapInfo.dropRate = 100;

        return mapInfo;
    }

    void EndGameLoot()
    {
        Debug.Log(dropRate);
        Debug.Log(dropAmount);
        if (Random.Range(0, 100) < dropRate)
        {
            gameCore.scoreManager.dropID = dropID;
            gameCore.scoreManager.dropAmmount = dropAmount;
        }
    }

    void InitialMapVision(int home_hang, int home_lie)
    {
        Hex Home = GameObject.Find("Map" + home_hang + "_" + home_lie).GetComponent<Hex>();
        for (int i = 0; i <= transform.childCount - 1; i++)
        {
            try
            {
                Hex ToCancelFog = transform.GetChild(i).GetComponent<Hex>();
                if (Vector3.Distance(Home.transform.position, ToCancelFog.transform.position) <= 17.5 * 2)
                {
                    ToCancelFog.isInFog = 999;
                    ToCancelFog.render = true;
                }
                //ToCancelFog.ChangeTheFog();
            }
            catch
            {
                continue;
            }
        }
    }
    public void FillUnitSlotWithPreset(Hex blueBox, int UnitID)
    {
        slots[0].spawnID = 1;
        slots[0].spawnUID = "沪造三六式十轮装甲车";
        //GameObject spawnedUnit;
        //int tempCounter = 0;
        //GameObject tiletoSpawn = GameObject.Find("Map" + blueBox.X + "_" + (blueBox.Z + 1));
        //Hex nextHex = tiletoSpawn.GetComponent<Hex>();
        //Score.SpawnDoll();
        //spawnedUnit = Instantiate(spawnSquad[UnitID].gameObject, tiletoSpawn.transform.position, Quaternion.identity);
        //Debug.Log("在" + tiletoSpawn.name + "生成了" + UnitID + "号单位" + spawnSquad[UnitID].name);
        //spawnedUnit.GetComponent<Unit>().hang = blueBox.X;
        //spawnedUnit.GetComponent<Unit>().lie = (blueBox.Z + 1);
        //spawnedUnit.GetComponent<DollsCombat>().allEnemy = enemyList;
        //spawnedUnit.GetComponent<DollsCombat>().allDolls = unitList;
        //spawnedUnit.GetComponent<DollsCombat>().thisUnit = spawnedUnit.GetComponent<Unit>();
        //spawnedUnit.GetComponent<DollsCombat>().map = this;

        ////spawnedUnit.GetComponent<DollsCombat>().FogOfWar();
        //nextHex.haveUnit = true;
        //spawnedUnit.transform.parent = unitList.transform;
        //Skills[UnitID].transform.SetParent(SkillSlot[tempCounter].transform);
        //tempCounter++;
        //Skills[UnitID].transform.localPosition = Vector3.zero;
        //IDollsSkillBehavior skill1 = Skills[UnitID].transform.GetChild(0).GetComponentInChildren<IDollsSkillBehavior>();
        //skill1.unit = spawnedUnit.GetComponent<DollsCombat>();
        //skill1.loadMap();
        //if (skill1.secondSkill != null)
        //{
        //    skill1.secondSkill.unit = spawnedUnit.GetComponent<DollsCombat>();
        //    skill1.secondSkill.loadMap();
        //    if (skill1.secondSkill.secondSkill != null)
        //    {
        //        skill1.secondSkill.secondSkill.unit = spawnedUnit.GetComponent<DollsCombat>();
        //        skill1.secondSkill.secondSkill.loadMap();
        //    }
        //}
        //spawnedUnit.GetComponent<DollsCombat>().CheckStatus();
    }

    public void SpawnTheUnits(int X, int Z)
    {
        GameObject spawnedUnit;
        int tempCounter = 0;
        for (int i = 0; i < 6; i++)//查看左右，上左上右，下左下右，周围的六个格子
        {
            int next_hang = X + changeX[i];
            int next_lie;
            if (X % 2 == 0)
            {
                next_lie = Z + changeZ[i];
            }
            else
            {
                next_lie = Z + changeZ[i + 6];
            }
            GameObject tiletoSpawn = GameObject.Find("Map" + next_hang + "_" + next_lie);
            Hex nextHex = tiletoSpawn.GetComponent<Hex>();
            Unit prefabUnit = null;
            Debug.Log(slots[i].spawnID);
            if (slots[i].spawnID != 0 && spawnSquardA.TryGetValue(slots[i].spawnUID, out prefabUnit))
            {
                gameCore.scoreManager.SpawnDoll();
                Debug.Log("test message1");
                spawnedUnit = Instantiate(prefabUnit.gameObject, tiletoSpawn.transform.position, Quaternion.identity);
                Debug.Log("在" + tiletoSpawn.name + "生成了" + slots[i].spawnUID + "号单位" + prefabUnit.name);
                spawnedUnit.GetComponent<Unit>().hang = next_hang;
                spawnedUnit.GetComponent<Unit>().lie = next_lie;
                spawnedUnit.GetComponent<DollsCombat>().allEnemy = enemyList;
                spawnedUnit.GetComponent<DollsCombat>().allDolls = unitList;
                spawnedUnit.GetComponent<DollsCombat>().thisUnit = spawnedUnit.GetComponent<Unit>();
                spawnedUnit.GetComponent<DollsCombat>().map = this;

                //spawnedUnit.GetComponent<DollsCombat>().FogOfWar();
                nextHex.haveUnit = true;
                spawnedUnit.transform.parent = unitList.transform;
                Skills[slots[i].spawnID].transform.SetParent(SkillSlot[tempCounter].transform);
                tempCounter++;
                Skills[slots[i].spawnID].transform.localPosition = Vector3.zero;
                IDollsSkillBehavior skill1 = Skills[slots[i].spawnID].transform.GetChild(0).GetComponentInChildren<IDollsSkillBehavior>();
                skill1.unit = spawnedUnit.GetComponent<DollsCombat>();
                //Skills[slots[i].spawnID].transform.GetChild(0).GetComponentInChildren<IDollsSkillBehavior>().mapList = this.gameObject;
                skill1.loadMap();
                if (skill1.secondSkill != null)
                {
                    skill1.secondSkill.unit = spawnedUnit.GetComponent<DollsCombat>();
                    skill1.secondSkill.loadMap();
                    if (skill1.secondSkill.secondSkill != null)
                    {
                        skill1.secondSkill.secondSkill.unit = spawnedUnit.GetComponent<DollsCombat>();
                        skill1.secondSkill.secondSkill.loadMap();
                    }
                }
                spawnedUnit.GetComponent<DollsCombat>().CheckStatus();
            }
        }
    }

    void SpawnTheEnemy(MapInfo mapInfo)
    {
        if (mapInfo.enemySpawnPoints != null)
        {
            for (int i = 0; i < mapInfo.enemySpawnPoints.Length; i++)
            {
                gameCore.scoreManager.SpawnEnemy();
                GameObject spawnedEnemy;
                if (mapInfo.enemySpawnPoints[i].spawnTile == "none")
                {
                    // 如果是不需要生成的敌方单位，直接跳到下一个。
                    continue;
                }
                GameObject tiletoSpawn = GameObject.Find(mapInfo.enemySpawnPoints[i].spawnTile);
                Hex Hex = tiletoSpawn.GetComponent<Hex>();
                spawnedEnemy = Instantiate(spawnEnemy[mapInfo.enemySpawnPoints[i].spawnType].gameObject, tiletoSpawn.transform.position, Quaternion.identity);
                EnemyCombat thisEnemy = spawnedEnemy.GetComponent<EnemyCombat>();
                thisEnemy.hang = Hex.X;
                thisEnemy.lie = Hex.Z;
                thisEnemy.map = gameObject;
                thisEnemy.dollsList = unitList;
                thisEnemy.targetHex = new Queue<Hex>();
                thisEnemy.moveWaitTime = new Queue<int>();
                thisEnemy.deScanTheMap = new Queue<Hex>();
                for (int j = 0; j < mapInfo.enemySpawnPoints[i].nextTile.Length; j++)
                {
                    thisEnemy.targetHex.Enqueue(GameObject.Find(mapInfo.enemySpawnPoints[i].nextTile[j]).GetComponent<Hex>());
                    thisEnemy.moveWaitTime.Enqueue(mapInfo.enemySpawnPoints[i].waitTime[j]);
                }
                spawnedEnemy.transform.parent = enemyList.transform;
                Hex.haveEnemy = true;
            }
        }
    }

    public bool IsBlocked(Hex point, Vector3 target)
    {
        try
        {
            Hex hex = null;
            bool blocked = false;
            //bool lastOne = false;
            if (FindDistance(point.gameObject, target) < 17.5)
            {
                return blocked;
            }
            int tempX, tempZ;
            float lastDistance = 9999f;
            Hex closestOne = null;
            int newX = point.X, newZ = point.Z;
            int k = 0;
            while (k < 100)
            {
                lastDistance = 9999f;
                for (int i = 0; i < 6; i++)
                {
                    if (newX % 2 == 0)
                    {
                        tempX = newX + changeX[i];
                        tempZ = newZ + changeZ[i];
                    }
                    else
                    {
                        tempX = newX + changeX[i];
                        tempZ = newZ + changeZ[i + 6];
                    }
                    if (tempX < maxX && tempZ < maxZ)
                    {
                        hex = transform.Find("Map" + tempX + "_" + tempZ).GetComponent<Hex>();
                        float distance = FindDistance(hex.gameObject, target);
                        if (distance <= lastDistance)
                        {
                            lastDistance = distance;
                            closestOne = hex;
                        }
                    }
                }
                newX = closestOne.X;
                newZ = closestOne.Z;
                if (closestOne.height > point.height || (closestOne.height == point.height && closestOne.blockVision))
                {
                    if (lastDistance <= 5)
                    {
                        break;
                    }
                    blocked = true;//如果遇到高地，咱们可以再来一格，比如山上我们是可以打到的，但是再来一个就不行了
                    break;
                }
                if (lastDistance <= 5)
                {
                    break;
                }
                k++;
            }
            return blocked;
        }
        catch
        {
            return false;
        }
    }

    public bool IsBlocked(Hex point, Hex target)
    {
        try
        {
            Hex hex = null;
            bool blocked = false;
            if (FindDistance(point.gameObject, target.gameObject) < 17.5)
            {
                return blocked;
            }
            int tempX, tempZ;
            float lastDistance = 9999f;
            Hex closestOne = null;
            int newX = point.X, newZ = point.Z;
            int k = 0;
            while (k < 500)
            {
                lastDistance = 9999f;
                for (int i = 0; i < 6; i++)
                {
                    if (newX % 2 == 0)
                    {
                        tempX = newX + changeX[i];
                        tempZ = newZ + changeZ[i];
                    }
                    else
                    {
                        tempX = newX + changeX[i];
                        tempZ = newZ + changeZ[i + 6];
                    }
                    if (tempX < maxX && tempZ < maxZ)
                    {
                        hex = transform.Find("Map" + tempX + "_" + tempZ).GetComponent<Hex>();
                        float distance = FindDistance(hex.gameObject, target.gameObject);
                        if (distance <= lastDistance)
                        {
                            lastDistance = distance;
                            closestOne = hex;
                        }
                    }
                }
                newX = closestOne.X;
                newZ = closestOne.Z;
                if (closestOne.height > point.height || (closestOne.height == point.height && closestOne.blockVision))
                {
                    if (lastDistance <= 5)
                    {
                        break;
                    }
                    blocked = true;//如果遇到高地，咱们可以再来一格，比如山上我们是可以打到的，但是再来一个就不行了
                    break;
                }
                if (lastDistance <= 5)
                {
                    break;
                }
                k++;
            }
            return blocked;
        }
        catch
        {
            return true;
        }

    }



    class MapGenerator
    {
        // generator parameters
        private MapGenre genre = MapGenre.Grassland;
        private bool riverEnabled = false;
        //
        private char[,] tiles = new char[0, 0];

        public MapGenerator() { }

        public enum MapGenre
		{
            Grassland,
            Desert,
            Snowfield,
            Forest,
            Muddy,
            Vocanic
		}

        public string[] generate(int lenHang, int lenLie)
		{
            this.tiles = new char[lenHang, lenLie];

            MyNoise myNoise = new MyNoise();

            // 使用Perlin Noise的实验生成
            float[,] noiseMap = myNoise.Perlin2D(lenLie + 2, lenHang + 2);
            float[,] tilesVal = myNoise.HexSample(noiseMap, new Rect(0, 0, lenLie / 3, lenHang), lenHang, lenLie);

            // 然后根据不同的preset来微调
            if (genre == MapGenre.Grassland)
            {
                // 首先填充草地
                fillWith('1');  // 1 - 草地

                float[,] forest = MyNoise.TopPercentageOf(tilesVal, 0.4f); // 4 - forest
                float[,] hills = MyNoise.TopPercentageOf(tilesVal, 0.2f); // 5 - hills
                float[,] mountain = MyNoise.TopPercentageOf(tilesVal, 0.08f); // 6 - hills

                //填充其他地形
                for (int i = 0; i < lenHang; i++)
                {
                    for (int j = 0; j < lenLie; j++)
                    {
                        if (false)
                        {
                            //nothing
                        }
                        else if (mountain[i, j] > 0f)
                        {
                            this.tiles[i, j] = '6';
                        }
                        else if (hills[i, j] > 0f)
                        {
                            this.tiles[i, j] = '5';
                        }
                        else if (forest[i, j] > 0f)
                        {
                            this.tiles[i, j] = '4';
                        }
                    }
                }
            }



            // （只是一个随机生成demo 目前还丑的要死）
            //         if (genre == MapGenre.Grassland)
            //{
            //             // 首先填充草地
            //             fillWith('1');  // 1 - 草地

            //             // 随机生成部分山脉
            //             // 随机噪音，+从中心到四周的gradient mask，x方向上拉伸采样
            //             float[,] noiseMap = myNoise.GetNoiseMap2D(lenLie + 2, lenHang + 2);
            //             float[,] tilesVal = myNoise.HexSample(noiseMap, new Rect(0, 0, lenLie / 3, lenHang), lenHang, lenLie);
            //             // 前12%是高地，前5%是山脉
            //             float[,] tile5 = MyNoise.TopPercentageOf(tilesVal, 0.12f);
            //             float[,] tile6 = MyNoise.TopPercentageOf(tilesVal, 0.05f);
            //             for (int i = 0; i < lenHang; i++)
            //	{
            //                 for (int j = 0; j < lenLie; j++)
            //                 {
            //                     if (tile6[i, j] > 0f)
            //                     {
            //                         this.tiles[i, j] = '6'; // 6 - 山脉
            //                     }
            //                     else if (tile5[i, j] > 0f)
            //			{
            //                         this.tiles[i, j] = '5'; // 5 - 高地
            //			}
            //		}
            //	}
            //}

            // 设置蓝色出生点
            this.tiles[5, 5] = 'B';

            // 填充 mapTiles
			// char[,] -> string[]
			string[] mapTiles = new string[lenHang];
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < lenHang; i++)
            {
                sb.Clear();
                for (int j = 0; j < lenLie; j++)
				{
                    sb.Append(tiles[i, j]);
				}
                mapTiles[i] = sb.ToString();
            }
            return mapTiles;
        }

        private void fillWith(char ch)
		{
            for(int i = 0; i < this.tiles.GetLength(0); i++)
			{
				for(int j = 0; j < this.tiles.GetLength(1); j++)
				{
                    this.tiles[i, j] = ch;
				}
			}

		}

        public MapGenerator setGenre(MapGenre genre)
		{
            this.genre = genre;
            return this;
		}

        public MapGenerator enableRiver(bool enabled)
		{
            this.riverEnabled = enabled;
            return this;
		}
    }
}