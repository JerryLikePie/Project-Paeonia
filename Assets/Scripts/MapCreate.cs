using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Utilities;

// todo �о�Ӧ�ø����� LevelCreator ֮��ģ�
public class MapCreate : MonoBehaviour
{
    //�Һ�tm�ʰ�����ģ���ţ������Ĳ����
    //���ǵú�ˮ������ˮ��ûζ��
    public List<TileType> tileTypes;
    public Unit[] spawnSquad = null;
    public USerializableDictionary<string, Unit> spawnSquardA;
    public EnemyProperty[] spawnEnemy = null;
    public GameObject[] SkillSlot;
    public GameObject[] Skills;
    public GameObject ObjectivePoint;
    public GameObject HomePoint;
    float zOffset = 8.655f;//�޺ۣ�8.65f���У�9f
    float xOffset = 17.31f;//�޺ۣ�17.31f���У�17.75f
    float hangOffset = 14.99f;//�޺ۣ�15f���У�15.35f
    public GameObject enemyList;
    public GameObject unitList;
    int homeHang = 0;
    int homeLie = 0;
    public float timeLimit;
    public int randomMapLimit;

    [HideInInspector] public Hex RedPoint, BluePoint;
    int maxX, maxZ;
    public SquadSlot[] slots;
    int dropID, dropAmount, dropRate;
    [HideInInspector] public string mapToLoad;

    // ��Ϸ�������
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
    // ���Գ������ɵ��˵����ɵ�
    class EnemyContinuousSpawnPoint
	{
        public string[] map;
        public float intensity;
        public Queue<int> spawnPool;
	}
    class MapInfo
    {
        public string[] mapTiles;
        public Queue<EnemySpawnPoint> enemySpawnPoints;
        public EnemyContinuousSpawnPoint enemyConinuousSpawnPoints;
        public int timeLimit;
        public int dropID;
        public int dropAmount;
        public int dropRate;

        public override string ToString()
        {
            string str = "";
            try
            {
                foreach (EnemySpawnPoint e in enemySpawnPoints)
                {
                    str += "spawnTile: " + e.spawnTile + "\n";
                    foreach (string s in e.nextTile)
                    {
                        str += "  nextTile: " + s + "\n";
                    }
                }
            }
            catch { }
            
            
            return str;
        }
    }

    class MapInfoLegacy
    {
        public string[] mapTiles;
        public EnemySpawnPoint[] enemySpawnPoints;
        public EnemyContinuousSpawnPoint enemyConinuousSpawnPoints;
        public int timeLimit;
        public int dropID;
        public int dropAmount;
        public int dropRate;

        public override string ToString()
        {
            string str = "";
            try
            {
                foreach (EnemySpawnPoint e in enemySpawnPoints)
                {
                    str += "spawnTile: " + e.spawnTile + "\n";
                    foreach (string s in e.nextTile)
                    {
                        str += "  nextTile: " + s + "\n";
                    }
                }
            }
            catch { }


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
        // ����ǽ̳�ģʽ��ʹ��Ԥ��
        // ����Ҫ��һ�ģ��ĳɲ�ͬ�̸̳���ͬԤ��
        FillUnitSlotWithPreset(BluePoint, 1);
        SpawnGame();
	}

    public void SpawnGame(bool generateRandomMap = false)
    {
        MapInfo mapInfo;

        //���ȣ����ɵ�ͼ����
        Debug.Log("map to load is: " + mapToLoad);
        if (!generateRandomMap)
        {
            // ���ص�ͼ�ļ�
            TextAsset textToMapJson = (TextAsset)Resources.Load(mapToLoad + "_json");
            mapInfo = JsonUtility.FromJson<MapInfo>(textToMapJson.text);
        }
        else
		{
            // ���������ͼ
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
                GameObject thisTile = tileTypes[7].tilePrefabType; //��Ĭ��Ϊ���������
                //Debug.Log(mapInfo.mapTiles[i][j]);
                if (mapInfo.mapTiles[i][j] == '0')//����ļ�˵������ˮ
                {
                    thisTile = tileTypes[0].tilePrefabType;
                }
                else if (mapInfo.mapTiles[i][j] == '1')//����ļ�˵�����ǲݵ�
                {
                    thisTile = tileTypes[1].tilePrefabType;
                }
                else if (mapInfo.mapTiles[i][j] == '2')//����ļ�˵������ɳ��
                {
                    thisTile = tileTypes[2].tilePrefabType;
                }
                else if (mapInfo.mapTiles[i][j] == '3')//����ļ�˵����������
                {
                    thisTile = tileTypes[3].tilePrefabType;
                }
                else if (mapInfo.mapTiles[i][j] == '4')//����ļ�˵����������
                {
                    thisTile = tileTypes[4].tilePrefabType;
                }
                else if (mapInfo.mapTiles[i][j] == '5')//����ļ�˵�����Ǹߵ�
                {
                    thisTile = tileTypes[5].tilePrefabType;
                }
                else if (mapInfo.mapTiles[i][j] == '6')//����ļ�˵������ɽ��
                {
                    thisTile = tileTypes[6].tilePrefabType;
                }
                else if (mapInfo.mapTiles[i][j] == 'B')//����ļ�˵�������ҷ��ļ�
                {
                    thisTile = tileTypes[8].tilePrefabType;
                    this.homeHang = i;
                    this.homeLie = j;
                }
                else if (mapInfo.mapTiles[i][j] == 'R')//����ļ�˵�����ǵз��ļ�
                {
                    thisTile = tileTypes[9].tilePrefabType;
                }
                else if (mapInfo.mapTiles[i][j] == 'L')//��ս��������ؿ飬ɢ����
                {
                    thisTile = tileTypes[10].tilePrefabType;
                }
                //��Ȼ����ֱ��tileTypes[mapFromText[i][j]]������ΪҪ�趨�ɵ��ﻹ���ƶ��������ɴ�ȫ�г�������
                //��ʲô�취�����Ż���

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
                //�������ͼ������
                thisTile.name = "Map" + i + "_" + j;
                try
                {
                    //������ֻ�Ǹ����ǿ��ģ�����ҲҪ֪��
                    thisTile.GetComponent<Hex>().X = i;
                    thisTile.GetComponent<Hex>().Z = j;
                    thisTile.GetComponent<Hex>().render = false;
                    if (mapInfo.mapTiles[i][j] == 'R')
                    {
                        this.RedPoint = thisTile.GetComponent<Hex>();
                        this.RedPoint.endGame = true;//����Ǻ��Ļ����൱�ڲȵ���������Ϸ����
                        this.ObjectivePoint = thisTile;
                    }
                    if (mapInfo.mapTiles[i][j] == 'B')
                    {
                        this.BluePoint = thisTile.GetComponent<Hex>();
                        this.BluePoint.endGame = true;//����Ǻ��Ļ����൱�ڲȵ���������Ϸ����
                        this.HomePoint = thisTile;
                        Camera.main.transform.position = HomePoint.transform.position + 78 * Vector3.up + 40 * Vector3.left + -78 * Vector3.forward;
                    }
                }
                catch
                { continue; }
                //�����е�ͼ�鶼װ��һ����ͼ�ļ�������
                thisTile.transform.parent = this.gameObject.transform;
            }
        }
        InitialMapVision(homeHang, homeLie);
        SpawnTheUnits(homeHang, homeLie);
        SpawnTheEnemy(mapInfo);
        //Debug.Log("��ͳ��ֱ�Ϊ" + maxX + "��" + maxZ);
    }
    
    private MapInfo GetGeneratedRandomMapInfo()
	{
        MapInfo mapInfo = new MapInfo();

        // todo ��ͼ���ɲ�������
        MapGenerator mapGen = new MapGenerator();
        mapGen.setGenre(MapGenerator.MapGenre.Forest).enableRiver(false);
        mapGen.setSize(randomMapLimit);

        // ���ɵ�ͼ
        string[,] temp = mapGen.generate();
        int tempsize = temp.GetLength(1);
        mapInfo.mapTiles = new string[tempsize];
        string[] mapMineOverlay = new string[tempsize];
        for (int i = 0; i < tempsize; i++)
        {
            mapInfo.mapTiles[i] = temp[0, i];
            mapMineOverlay[i] = temp[1, i];
        }
        

        // todo ���Գ������ɵ�
        EnemyContinuousSpawnPoint continuousSpawnPoint = new EnemyContinuousSpawnPoint();
        continuousSpawnPoint.map = mapInfo.mapTiles;
        continuousSpawnPoint.spawnPool = new Queue<int>();
        continuousSpawnPoint.spawnPool.Enqueue(1);

        // todo ���������ɵĵ�ͼ�Ŀɴ���

        // ���ɿ���
        mapInfo.enemySpawnPoints = new Queue<EnemySpawnPoint>();
        SpawnTheMinerals(mapInfo, mapMineOverlay);


        // ���ɵ���
        // �������ɵ��߼����£�
        // ���ŵ������Զ���ߣ����˻��δ����������������ɳ�������
        // ��ͬ�ĵ����в�ͬ����ʼ��Ծ�ȣ�����Ծ��Ҳ�����������ɣ����ǽ���ʽ�������
        // ��ʹ����ڸ���ʼ��Ծ�ȵĵ�ͼ�ϣ�һ��ʼ���ܾ��й���

        //mapInfo.enemySpawnPoints = new EnemySpawnPoint[0];
        //mapInfo.enemyConinuousSpawnPoints = new EnemyContinuousSpawnPoint;
        //mapInfo.timeLimit = 1200;
        //mapInfo.dropID = 1;
        //mapInfo.dropAmount = 15;
        //mapInfo.dropRate = 100;

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
                ToCancelFog.isInFog = 999; //�����ã�ֱ����ʾ���е�ͼ
                ToCancelFog.render = true; //�����ã�ֱ����ʾ���е�ͼ
            }
            catch
            {
                continue;
            }
        }
    }
    public void FillUnitSlotWithPreset(Hex blueBox, int UnitID)
    {
        slots[0].spawnID = UnitID;
        slots[0].spawnUID = "��������ʽʮ��װ�׳�";
    }

    public void SpawnTheUnits(int X, int Z)
    {
        GameObject spawnedUnit;
        int tempCounter = 0;
        for (int i = 0; i < 6; i++)//�鿴���ң��������ң��������ң���Χ����������
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
                Debug.Log("��" + tiletoSpawn.name + "������" + slots[i].spawnUID + "�ŵ�λ" + prefabUnit.name);
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
            for (int i = 0; i < mapInfo.enemySpawnPoints.Count; i++)
            {
                // �����ˡ�ʵ�������п��Ա��ҷ���ɫ��Ե�һ������
                // ��ˣ������ˡ�������治ֻ�ез���ɫ��Ҳ�пɱ��ҷ�ռ���������ʩ�Ϳɱ��ɼ�����Դ
                // 0-99�����˵з���ɫ��100+������������ʩ����Դ��
                if (mapInfo.enemySpawnPoints.Peek().spawnType >= 100)
                {
                    // ��ô�������Դ������
                    // ��Դ�����޷��ƶ��ģ�������ȻҲ����scan map֮��
                    GameObject spawnedEnemy;
                    if (mapInfo.enemySpawnPoints.Peek().spawnTile == "none")
                    {
                        // ����ǲ���Ҫ���ɵĵз���λ��ֱ��������һ����
                        mapInfo.enemySpawnPoints.Dequeue();
                        continue;
                    }
                    GameObject tiletoSpawn = GameObject.Find(mapInfo.enemySpawnPoints.Peek().spawnTile);
                    Hex Hex = tiletoSpawn.GetComponent<Hex>();
                    spawnedEnemy = Instantiate(spawnEnemy[mapInfo.enemySpawnPoints.Peek().spawnType].gameObject, tiletoSpawn.transform.position, Quaternion.identity);
                    EnemyCombat thisEnemy = spawnedEnemy.GetComponent<EnemyCombat>();
                    thisEnemy.hang = Hex.X;
                    thisEnemy.lie = Hex.Z;
                    thisEnemy.map = gameObject;
                    thisEnemy.dollsList = unitList;
                    spawnedEnemy.transform.parent = enemyList.transform;
                }
                else
                {
                    // �ǵз���λ������
                    gameCore.scoreManager.SpawnEnemy();
                    GameObject spawnedEnemy;
                    if (mapInfo.enemySpawnPoints.Peek().spawnTile == "none")
                    {
                        // ����ǲ���Ҫ���ɵĵз���λ��ֱ��������һ����
                        mapInfo.enemySpawnPoints.Dequeue();
                        continue;
                    }
                    GameObject tiletoSpawn = GameObject.Find(mapInfo.enemySpawnPoints.Peek().spawnTile);
                    Hex Hex = tiletoSpawn.GetComponent<Hex>();
                    spawnedEnemy = Instantiate(spawnEnemy[mapInfo.enemySpawnPoints.Peek().spawnType].gameObject, tiletoSpawn.transform.position, Quaternion.identity);
                    EnemyCombat thisEnemy = spawnedEnemy.GetComponent<EnemyCombat>();
                    thisEnemy.hang = Hex.X;
                    thisEnemy.lie = Hex.Z;
                    thisEnemy.map = gameObject;
                    thisEnemy.dollsList = unitList;
                    thisEnemy.targetHex = new Queue<Hex>();
                    thisEnemy.moveWaitTime = new Queue<int>();
                    thisEnemy.deScanTheMap = new Queue<Hex>();
                    for (int j = 0; j < mapInfo.enemySpawnPoints.Peek().nextTile.Length; j++)
                    {
                        thisEnemy.targetHex.Enqueue(GameObject.Find(mapInfo.enemySpawnPoints.Peek().nextTile[j]).GetComponent<Hex>());
                        thisEnemy.moveWaitTime.Enqueue(mapInfo.enemySpawnPoints.Peek().waitTime[j]);
                    }
                    spawnedEnemy.transform.parent = enemyList.transform;
                    Hex.haveEnemy = true;
                }
                mapInfo.enemySpawnPoints.Dequeue();
            }
        }
    }

    void SpawnTheMinerals(MapInfo mapinfo, string[] minerals)
    {
        // �����ͼ�Ѿ����ŵ�ͼ��������
        for (int i = 0; i < randomMapLimit; i++)
        {
            for (int j = 0; j < randomMapLimit; j++)
            {
                if (minerals[i][j] == '1')
                {
                    EnemySpawnPoint thisMineral = new EnemySpawnPoint();
                    // TODO ����������100���һ��������Ϳ�����һ���������ɲ�ͬ����Ŀ���
                    thisMineral.spawnType = 100;
                    thisMineral.spawnTile = "Map" + i + "_" + j;
                    Debug.Log(thisMineral.spawnTile);
                    mapinfo.enemySpawnPoints.Enqueue(thisMineral);
                }
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
                    blocked = true;//��������ߵأ����ǿ�������һ�񣬱���ɽ�������ǿ��Դ򵽵ģ���������һ���Ͳ�����
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
                    blocked = true;//��������ߵأ����ǿ�������һ�񣬱���ɽ�������ǿ��Դ򵽵ģ���������һ���Ͳ�����
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
        private int size = 30;
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

        public string[,] generate()
		{
            char basicTile = '1';
            this.tiles = new char[size, size];

            MyNoise myNoise = new MyNoise();

            // ʹ��Perlin Noise��ʵ������
            float[,] noiseMap = myNoise.Perlin2D(size + 2, size + 2, 3f);
            float[,] tilesVal = myNoise.HexSample(noiseMap, new Rect(0, 0, size, size), size, size);

            // Ȼ����ݲ�ͬ��preset��΢��
            if (genre == MapGenre.Grassland)
            {
                // ��ԭ��
                // �������ݵ�
                basicTile = '1'; // 1 - �ݵ�
                fillWith(basicTile);

                float[,] forest = MyNoise.TopPercentageOf(tilesVal, 0.35f); // 4 - forest
                float[,] hills = MyNoise.TopPercentageOf(tilesVal, 0.15f); // 5 - hills
                float[,] mountain = MyNoise.TopPercentageOf(tilesVal, 0.03f); // 6 - mountain
                float[,] river = MyNoise.GenerateRiver(tilesVal, false, 1); // ����

                //�����������
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        if (false)
                        {
                            Debug.LogError("why are you here?");
                        }
                        else if (river[i, j] > 0f)
                        {
                            this.tiles[i, j] = '0';
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
            else if (genre == MapGenre.Desert)
            {
                // ɳĮ��
                basicTile = '2'; // 2 - ɳ��
                fillWith(basicTile);

                float[,] hills = MyNoise.TopPercentageOf(tilesVal, 0.2f); // 5 - hills
                float[,] mountain = MyNoise.TopPercentageOf(tilesVal, 0.03f); // 6 - mountains
                float[,] river = MyNoise.GenerateRiver(tilesVal, true, 1); // ����

                //�����������
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        if (false)
                        {
                            //nothing
                        }
                        else if (river[i, j] > 0f)
                        {
                            this.tiles[i, j] = '0';
                        }
                        else if (mountain[i, j] > 0f)
                        {
                            this.tiles[i, j] = '6';
                        }
                        else if (hills[i, j] > 0f)
                        {
                            this.tiles[i, j] = '5';
                        }
                    }
                }
            } 
            else if (genre == MapGenre.Muddy)
            {
                // ����
                basicTile = '3'; // 3 - �����
                fillWith(basicTile);

                float[,] land = MyNoise.TopPercentageOf(tilesVal, 0.85f); // all lower are water
                float[,] grass = MyNoise.TopPercentageOf(tilesVal, 0.5f); // 4 - grass
                float[,] forest = MyNoise.TopPercentageOf(tilesVal, 0.1f); // 4 - forest
                float[,] river = MyNoise.GenerateRiver(tilesVal, true, 1); // ����

                //�����������
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        if (false)
                        {
                            //nothing
                        }
                        else if (river[i, j] > 0f)
                        {
                            //this.tiles[i, j] = '0';
                        }
                        else if (forest[i, j] > 0f)
                        {
                            this.tiles[i, j] = '4';
                        }
                        else if (grass[i, j] > 0f)
                        {
                            this.tiles[i, j] = '1';
                        }
                        else if (land[i, j] > 0f)
                        {
                            this.tiles[i, j] = '3';
                        }
                        else
                        {
                            this.tiles[i, j] = '0';
                        }
                    }
                }
            }
            else if (genre == MapGenre.Forest)
            {
                // ���֣�
                // �������ݵ�
                basicTile = '1'; // 1 - �ݵ�
                fillWith(basicTile);

                float[,] forest = MyNoise.TopPercentageOf(tilesVal, 0.8f); // 4 - forest
                float[,] hills = MyNoise.TopPercentageOf(tilesVal, 0.1f); // 5 - hills
                float[,] mountain = MyNoise.TopPercentageOf(tilesVal, 0.03f); // 6 - mountain
                float[,] river = MyNoise.GenerateRiver(tilesVal, false, 1); // ����

                //�����������
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        if (false)
                        {
                            Debug.LogError("why are you here?");
                        }
                        else if (river[i, j] > 0f)
                        {
                            this.tiles[i, j] = '0';
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

            // ���ɿ���
            char[,] mineral = new char[size, size];
            // ����ѡ��һ�����������
            int cx = 5;
            int cy = 5;
            do
            {
                cx = Random.Range(0, size);
                cy = Random.Range(0, size);
            } 
            while (this.tiles[cx, cy] == '0');
            mineral[cx, cy] = '1';
            // Ȼ�����Ÿÿ�������������
            float maxDistance = size / 1.5f;
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    if (x == cx && y == cy) continue;

                    float dx = x - cx;
                    float dy = y - cy;
                    float distance = Mathf.Sqrt(dx * dx + dy * dy);

                    float outwardFalloff = Mathf.Clamp01(1 - (distance / maxDistance));
                    float maxNoise = Mathf.Max(noiseMap[x, y]);
                    float perlinInfluence = noiseMap[x, y] / maxNoise / 1.35f;
                    float probability = outwardFalloff * perlinInfluence;

                    if (Random.value < probability)
                    {
                        if (this.tiles[x,y] != '0')
                        {
                            mineral[x, y] = '1';
                        }
                    }
                    else
                    {
                        mineral[x, y] = '0';
                    }
                }
            }


            // ������ɫ������
            this.tiles[size / 2, size / 2] = 'B';
            for (int i = 0; i < 6; i++)//�鿴���ң��������ң��������ң���Χ����������
            {
                int next_hang = size / 2 + changeX[i];
                int next_lie;
                if ((size / 2) % 2 == 0)
                {
                    next_lie = (size / 2) + changeZ[i];
                }
                else
                {
                    next_lie = (size / 2) + changeZ[i + 6];
                }
                this.tiles[next_hang, next_lie] = basicTile; // �ĳ�����趨�µĻ����ؿ�
            }

            // ��� mapTiles
            // char[,] -> string[]
            string[,] mapTiles = new string[2,size];
            StringBuilder sb = new StringBuilder();
            // ���������ͼ����Ϣ
            for (int i = 0; i < size; i++)
            {
                sb.Clear();
                for (int j = 0; j < size; j++)
				{
                    sb.Append(tiles[i, j]);
				}
                mapTiles[0,i] = sb.ToString();
            }
            // Ȼ������������Ϣ
            for (int i = 0; i < size; i++)
            {
                sb.Clear();
                for (int j = 0; j < size; j++)
                {
                    sb.Append(mineral[i, j]);
                }
                mapTiles[1, i] = sb.ToString();
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

        public MapGenerator setSize(int size)
        {
            this.size = size;
            return this;
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