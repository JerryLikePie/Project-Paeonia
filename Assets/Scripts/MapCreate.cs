using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Utilities;

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
    void Start()
    {
        mapToLoad = PlayerPrefs.GetString("Stage_You_Should_Load", "Map_1-1");
    }

    public bool IsTutorial()
    {
        return mapToLoad.Contains("TR");
    }

    public void SpawnGameWithPreset()
    {
        // ����ǽ̳�ģʽ��ʹ��Ԥ��
        // ����Ҫ��һ�ģ��ĳɲ�ͬ�̸̳���ͬԤ��
        FillUnitSlotWithPreset(BluePoint, 1);
        SpawnGame();
	}

    public void SpawnGame()
    {
        MapInfo mapInfo;

        //���ȣ����ɵ�ͼ����
        Debug.Log(mapToLoad);
        TextAsset textToMapJson = (TextAsset)Resources.Load(mapToLoad + "_json");
        mapInfo = JsonUtility.FromJson<MapInfo>(textToMapJson.text);
        Debug.Log("mapInfo:\n" + mapInfo);
        timeLimit = mapInfo.timeLimit;
        maxZ = mapInfo.mapTiles.Length;
        maxX = mapInfo.mapTiles[0].Length;
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
                    thisTile.GetComponent<Hex>().X = i;
                    thisTile.GetComponent<Hex>().Z = j;
                    thisTile.GetComponent<Hex>().render = false;
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
    
    // Update is called once per frame
    void Update()
    {

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
        slots[0].spawnUID = "��������ʽʮ��װ�׳�";
        //GameObject spawnedUnit;
        //int tempCounter = 0;
        //GameObject tiletoSpawn = GameObject.Find("Map" + blueBox.X + "_" + (blueBox.Z + 1));
        //Hex nextHex = tiletoSpawn.GetComponent<Hex>();
        //Score.SpawnDoll();
        //spawnedUnit = Instantiate(spawnSquad[UnitID].gameObject, tiletoSpawn.transform.position, Quaternion.identity);
        //Debug.Log("��" + tiletoSpawn.name + "������" + UnitID + "�ŵ�λ" + spawnSquad[UnitID].name);
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
            for (int i = 0; i < mapInfo.enemySpawnPoints.Length; i++)
            {
                gameCore.scoreManager.SpawnEnemy();
                GameObject spawnedEnemy;
                if (mapInfo.enemySpawnPoints[i].spawnTile == "none")
                {
                    // ����ǲ���Ҫ���ɵĵз���λ��ֱ��������һ����
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
}