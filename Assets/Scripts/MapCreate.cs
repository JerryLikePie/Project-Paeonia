using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class MapCreate : MonoBehaviour
{
    //我好tm渴啊，真的，喝牛奶是真的不解渴
    //还是得喝水，但是水又没味道
    public List<TileType> tileTypes;
    public Unit[] spawnSquad = null;
    public EnemyProperty[] spawnEnemy = null;
    public GameObject[] Skills;
    public GameObject ObjectivePoint;
    public GameObject HomePoint;
    //public GameObject hexPrefab;
    [SerializeField] int mapToLoad;
    float zOffset = 8.65f;//无痕：8.65f，有：9f
    float xOffset = 17.31f;//无痕：17.31f，有：17.75f
    float hangOffset = 15f;//无痕：15f，有：15.35f
    // Start is called before the first frame update
    public GameObject enemyList;
    public GameObject unitList;
    int homeHang = 0;
    int homeLie = 0;
    public GameObject ScoreManager;
    public ScoreManager Score;
    long timeStart;
    public float timeLimit;
    Hex RedPoint, BluePoint;

    public void SpawnGameMap()
    {
        timeStart = System.DateTime.Now.Ticks;
        MapOverlayCreate overlay = gameObject.GetComponentInChildren<MapOverlayCreate>();
        //首先，生成地图本体
        TextAsset textToMap = (TextAsset)Resources.Load("Map" + mapToLoad);
        string[] mapFromText = textToMap.text.Trim().Split('\n');
        TextAsset textToMapOverlay = (TextAsset)Resources.Load("MapOverlay" + mapToLoad);
        string[] mapOverlayFromText = textToMapOverlay.text.Trim().Split('\n');

        for (int i = 0; i < mapFromText.Length; i++)
        {
            for (int j = 0; j < mapFromText[i].Length - 1; j++)
            {
                GameObject thisTile = tileTypes[7].tilePrefabType; //先默认为都生成虚空
                if (mapFromText[i][j] == '0')//如果文件说这里是水
                {
                    thisTile = tileTypes[0].tilePrefabType;
                }
                else if (mapFromText[i][j] == '1')//如果文件说这里是草地
                {
                    thisTile = tileTypes[1].tilePrefabType;
                }
                else if (mapFromText[i][j] == '2')//如果文件说这里是沙地
                {
                    thisTile = tileTypes[2].tilePrefabType;
                }
                else if (mapFromText[i][j] == '3')//如果文件说这里是沼泽
                {
                    thisTile = tileTypes[3].tilePrefabType;
                }
                else if (mapFromText[i][j] == '4')//如果文件说这里是树林
                {
                    thisTile = tileTypes[4].tilePrefabType;
                }
                else if (mapFromText[i][j] == '5')//如果文件说这里是高地
                {
                    thisTile = tileTypes[5].tilePrefabType;
                }
                else if (mapFromText[i][j] == '6')//如果文件说这里是山地
                {
                    thisTile = tileTypes[6].tilePrefabType;
                }
                else if (mapFromText[i][j] == 'B')//如果文件说这里是我方的家
                {
                    thisTile = tileTypes[8].tilePrefabType;
                    homeHang = i;
                    homeLie = j;
                }
                else if (mapFromText[i][j] == 'R')//如果文件说这里是敌方的家
                {
                    thisTile = tileTypes[9].tilePrefabType;
                }
                //虽然可以直接tileTypes[mapFromText[i][j]]但是因为要设定可到达还有移动点数，干脆全列出来算了
                //有什么办法可以优化吗？
                GameObject obj;
                if (i % 2 == 0)
                {
                    obj = Instantiate(thisTile, new Vector3(j * xOffset, 0, i * hangOffset), Quaternion.identity);
                }
                else
                {
                    obj = Instantiate(thisTile, new Vector3((j * xOffset) - zOffset, 0, i * hangOffset), Quaternion.identity);
                }
                //给这个地图命名
                obj.name = "Map" + i + "_" + j;
                try
                {
                    //但命名只是给我们看的，程序也要知道
                    obj.GetComponent<Hex>().hang = i;
                    obj.GetComponent<Hex>().lie = j;
                    if (mapOverlayFromText[i][j] == 'R')
                    {
                        RedPoint = obj.GetComponent<Hex>();
                        RedPoint.endGame = true;//如果是红点的话，相当于踩到这个点就游戏结束
                        ObjectivePoint = obj;
                    }
                    if (mapOverlayFromText[i][j] == 'B')
                    {
                        BluePoint = obj.GetComponent<Hex>();
                        BluePoint.endGame = true;//如果是红点的话，相当于踩到这个点就游戏结束
                        HomePoint = obj;
                    }
                }
                catch
                { continue; }
                //把所有地图块都装入一个地图文件夹里面
                obj.transform.parent = this.gameObject.transform;
            }
        }
        InitialMapVision(homeHang, homeLie);
        SpawnTheEnemy("Map5_8");
        SpawnTheEnemy("Map6_10");
        SpawnTheEnemy("Map7_10");
        SpawnTheEnemy("Map6_11");
        SpawnTheEnemy("Map6_9");
        SpawnTheEnemy("Map7_12");
        SpawnTheEnemy("Map9_12");
        SpawnTheEnemy("Map1_12");
        SpawnTheEnemy("Map10_12");
        //SpawnTheEnemy("Map9_13");
        SpawnTheUnits(homeHang, homeLie);
        //StartCoroutine(SecondWave());
    }
    void Start()
    {
        Score = ScoreManager.GetComponent<ScoreManager>();
        Score.Initialize();
    }
    IEnumerator SecondWave()
    {
        yield return new WaitForSeconds(60f);
        //SpawnTheEnemy("Map12_2");
        //SpawnTheEnemy("Map8_10");
        //SpawnTheEnemy("Map8_8");
        //SpawnTheEnemy("Map9_13");
        //SpawnTheEnemy("Map10_12");
        //SpawnTheEnemy("Map11_11");
        //SpawnTheEnemy("Map12_9");
    }
    // Update is called once per frame
    void Update()
    {
        if (ObjectivePoint != null && RedPoint.haveUnit == true)
        {
            Score.enemyBaseCaptured = true;
            Score.CapturedEnemyPoints();
            Invoke("GameEnd", 3f);
        }
        if (HomePoint != null && BluePoint.haveEnemy == true)
        {
            Score.friendlyBaseCaptured = true;
            Invoke("GameEnd", 3f);
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
                       1, 0, -1, -1, - 1, 0 };//分奇偶层，当前坐标的“六个可能的下个坐标”
        GameObject spawnedUnit;
        SquadSlot slot;
        SquadSelectionPage gameInitial;
        gameInitial = GameObject.Find("SquadSelection").GetComponent<SquadSelectionPage>();
        for (int i = 1; i <= 6; i++)//查看左右，上左上右，下左下右，周围的六个格子
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
                Debug.Log("在" + tiletoSpawn.name + "生成了" + slot.spawnID + "号单位" + spawnSquad[slot.spawnID].name);
                spawnedUnit.GetComponent<Unit>().hang = next_hang;
                spawnedUnit.GetComponent<Unit>().lie = next_lie;
                spawnedUnit.GetComponent<DollsCombat>().enemiesList = enemyList;
                spawnedUnit.GetComponent<DollsCombat>().dollsList = unitList;
                spawnedUnit.GetComponent<DollsCombat>().map = this.gameObject;
                nextHex.haveUnit = true;
                spawnedUnit.transform.parent = unitList.transform;
                spawnedUnit.GetComponent<DollsCombat>().FogOfWar();
                Skills[slot.spawnID].SetActive(true);
                Skills[slot.spawnID].transform.GetChild(0).GetComponentInChildren<IDollsSkillBehavior>().unit = spawnedUnit.GetComponent<DollsCombat>();
                Skills[slot.spawnID].transform.GetChild(0).GetComponentInChildren<IDollsSkillBehavior>().mapList = this.gameObject;
                Skills[slot.spawnID].transform.GetChild(0).GetComponentInChildren<IDollsSkillBehavior>().loadMap();
            }
        }
        gameInitial.gameObject.SetActive(false);
    }
    public void SpawnTheEnemy(string map)
    {
        Score.totalEnemy += 1;
        GameObject spawnedEnemy;
        GameObject tiletoSpawn = GameObject.Find(map);
        Hex Hex = tiletoSpawn.GetComponent<Hex>();
        spawnedEnemy = Instantiate(spawnEnemy[(int)Random.Range(0f,1.5f)].gameObject, tiletoSpawn.transform.position, Quaternion.identity);
        spawnedEnemy.GetComponent<EnemyCombat>().hang = Hex.hang;
        spawnedEnemy.GetComponent<EnemyCombat>().lie = Hex.lie;
        spawnedEnemy.GetComponent<EnemyCombat>().map = gameObject;
        spawnedEnemy.GetComponent<EnemyCombat>().dollsList = unitList;
        spawnedEnemy.GetComponent<EnemyCombat>().targetHex = GameObject.Find("Map" + homeHang + "_" + homeLie).GetComponent<Hex>();
        spawnedEnemy.transform.parent = enemyList.transform;
        Hex.haveEnemy = true;
    }
}
