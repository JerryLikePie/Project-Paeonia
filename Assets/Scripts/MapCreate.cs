using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class MapCreate : MonoBehaviour
{
    //�Һ�tm�ʰ�����ģ���ţ������Ĳ����
    //���ǵú�ˮ������ˮ��ûζ��
    public List<TileType> tileTypes;
    public Unit[] spawnSquad = null;
    public EnemyProperty[] spawnEnemy = null;
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
    public GameObject ScoreManager;
    public ScoreManager Score;
    long timeStart;
    public float timeLimit;
    Hex RedPoint, BluePoint;

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

    public void SpawnGameMap()
    {
        MapInfo mapInfo;
        timeStart = System.DateTime.Now.Ticks;
        //���ȣ����ɵ�ͼ����
        string mapToLoad = PlayerPrefs.GetString("Stage_You_Should_Load", "Map_2-1");
        Debug.Log(mapToLoad);
        TextAsset textToMapJson = (TextAsset)Resources.Load(mapToLoad + "_json");
        mapInfo = JsonUtility.FromJson<MapInfo>(textToMapJson.text);
        Debug.Log("mapInfo:\n" + mapInfo);
        timeLimit = mapInfo.timeLimit;
        for (int i = 0; i < mapInfo.mapTiles.Length; i++)
        {
            for (int j = 0; j < mapInfo.mapTiles[i].Length - 1; j++)
            {
                GameObject thisTile = tileTypes[7].tilePrefabType; //��Ĭ��Ϊ���������
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
                    homeHang = i;
                    homeLie = j;
                }
                else if (mapInfo.mapTiles[i][j] == 'R')//����ļ�˵�����ǵз��ļ�
                {
                    thisTile = tileTypes[9].tilePrefabType;
                }
                //��Ȼ����ֱ��tileTypes[mapFromText[i][j]]������ΪҪ�趨�ɵ��ﻹ���ƶ��������ɴ�ȫ�г�������
                //��ʲô�취�����Ż���
                
                if (i % 2 == 0)
                {
                    thisTile = Instantiate(thisTile, new Vector3(j * xOffset, 0, i * hangOffset), Quaternion.identity);
                }
                else
                {
                    thisTile = Instantiate(thisTile, new Vector3((j * xOffset) - zOffset, 0, i * hangOffset), Quaternion.identity);
                }
                //�������ͼ����
                thisTile.name = "Map" + i + "_" + j;
                try
                {
                    //������ֻ�Ǹ����ǿ��ģ�����ҲҪ֪��
                    thisTile.GetComponent<Hex>().hang = i;
                    thisTile.GetComponent<Hex>().lie = j;
                    if (mapInfo.mapTiles[i][j] == 'R')
                    {
                        RedPoint = thisTile.GetComponent<Hex>();
                        RedPoint.endGame = true;//����Ǻ��Ļ����൱�ڲȵ���������Ϸ����
                        ObjectivePoint = thisTile;
                    }
                    if (mapInfo.mapTiles[i][j] == 'B')
                    {
                        BluePoint = thisTile.GetComponent<Hex>();
                        BluePoint.endGame = true;//����Ǻ��Ļ����൱�ڲȵ���������Ϸ����
                        HomePoint = thisTile;
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
    }
    void Start()
    {
        Score = ScoreManager.GetComponent<ScoreManager>();
        Score.Initialize();
        Score.stageName = PlayerPrefs.GetString("Stage_You_Should_Load", null);
    }
    // Update is called once per frame
    void Update()
    {
        if (ObjectivePoint != null && RedPoint.haveUnit == true)
        {
            Debug.Log("�����Ѿ�����������ô����ת�أ�����");
            Score.enemyBaseCaptured = true;
            Score.CapturedEnemyPoints();
            Invoke("GameEnd", 2);
            GameEnd();
        }
        if (HomePoint != null && BluePoint.haveEnemy == true)
        {
            Score.friendlyBaseCaptured = true;
            Invoke("GameEnd", 2);
        }
    }
    void GameEnd()
    {
        Score.timeTook = (System.DateTime.Now.Ticks - timeStart)/10000000f;
        Score.timeLimit = timeLimit;
        Score.GameEnded();
        SceneManager.LoadScene("GameEnd");
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
                }
                //ToCancelFog.ChangeTheFog();
            }
            catch
            {
                continue;
            }
        }
    }
    public void SpawnTheUnits(int current_hang, int current_lie)
    {
        int[] change_hang = { 0, 1, 1, 0, -1, -1 };
        int[] change_lie = { 1, 1, 0, -1 , 0, 1,
                       1, 0, -1, -1, - 1, 0 };//����ż�㣬��ǰ����ġ��������ܵ��¸����ꡱ
        GameObject spawnedUnit;
        SquadSlot slot;
        SquadSelectionPage gameInitial;
        gameInitial = GameObject.Find("SquadSelection").GetComponent<SquadSelectionPage>();
        for (int i = 1; i <= 6; i++)//�鿴���ң��������ң��������ң���Χ����������
        {
            slot = GameObject.Find("Slot" + i).GetComponent<SquadSlot>();
            int next_hang = current_hang + change_hang[i - 1];
            int next_lie;
            if (current_hang % 2 == 0)
            {
                next_lie = current_lie + change_lie[i - 1];
            }
            else
            {
                next_lie = current_lie + change_lie[i + 6 - 1];
            }
            GameObject tiletoSpawn = GameObject.Find("Map" + next_hang + "_" + next_lie);
            Hex nextHex = tiletoSpawn.GetComponent<Hex>();
            if (slot.spawnID != 0 && spawnSquad[slot.spawnID] != null)
            {
                Score.totalDolls++;
                spawnedUnit = Instantiate(spawnSquad[slot.spawnID].gameObject, tiletoSpawn.transform.position, Quaternion.identity);
                Debug.Log("��" + tiletoSpawn.name + "������" + slot.spawnID + "�ŵ�λ" + spawnSquad[slot.spawnID].name);
                spawnedUnit.GetComponent<Unit>().hang = next_hang;
                spawnedUnit.GetComponent<Unit>().lie = next_lie;
                spawnedUnit.GetComponent<DollsCombat>().allEnemy = enemyList;
                spawnedUnit.GetComponent<DollsCombat>().allDolls = unitList;
                spawnedUnit.GetComponent<DollsCombat>().map = this.gameObject;
                nextHex.haveUnit = true;
                spawnedUnit.transform.parent = unitList.transform;
                //spawnedUnit.GetComponent<DollsCombat>().FogOfWar();
                Skills[slot.spawnID].SetActive(true);
                Skills[slot.spawnID].transform.GetChild(0).GetComponentInChildren<IDollsSkillBehavior>().unit = spawnedUnit.GetComponent<DollsCombat>();
                Skills[slot.spawnID].transform.GetChild(0).GetComponentInChildren<IDollsSkillBehavior>().mapList = this.gameObject;
                Skills[slot.spawnID].transform.GetChild(0).GetComponentInChildren<IDollsSkillBehavior>().loadMap();
            }
        }
        gameInitial.gameObject.SetActive(false);
    }

    void SpawnTheEnemy(MapInfo mapInfo)
    {
        if (mapInfo.enemySpawnPoints != null)
        {
            for (int i = 0; i < mapInfo.enemySpawnPoints.Length; i++)
            {
                Score.totalEnemy += 1;
                GameObject spawnedEnemy;
                GameObject tiletoSpawn = GameObject.Find(mapInfo.enemySpawnPoints[i].spawnTile);
                Hex Hex = tiletoSpawn.GetComponent<Hex>();
                spawnedEnemy = Instantiate(spawnEnemy[mapInfo.enemySpawnPoints[i].spawnType].gameObject, tiletoSpawn.transform.position, Quaternion.identity);
                EnemyCombat thisEnemy = spawnedEnemy.GetComponent<EnemyCombat>();
                thisEnemy.hang = Hex.hang;
                thisEnemy.lie = Hex.lie;
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
}
