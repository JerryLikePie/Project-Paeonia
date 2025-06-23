using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    int killedEnemy, totalEnemy, killedDolls, totalDolls;
    bool enemyBaseCaptured = false, friendlyBaseCaptured = false;
    float finalPoints, killPoints, capturePoints, timePoints, casualtyPoints, timeTook, timeLimit;
    public GameObject enemyList;
    public GameObject friendlyList;
    public string stageName;
    [SerializeField] Text killCount;
    [SerializeField] Text timeCount;
    [SerializeField] Text dangerLevel;
    public GameObject HUD;
    public int dropID;
    public int dropAmmount;

    public GameCore gameCore;

    float timeTick;
    float timeSec;

    // 用于跨场景传递的数据包
    public struct GameScoreInfo
    {
		public bool inTime;             // 在限定时间内胜利
        public bool captureObjective;   // 占领目标
        public bool noDeath;            // 没有阵亡
        public bool allDestroyed;       // 摧毁全部敌方单位
        public bool lost;               // 是否失败

        public float timeTook;          // 游戏时长
        public float timeLimit;         // 关卡限定时长

        public string stageName;        // 关卡ID
    }

	void Start()
	{
        // 监听敌方单位被击杀事件
        gameCore.eventSystem.RegistListener(GameEventType.Event_Enemy_Killed, OnEnemyKilled);
        // 监听矿物被开采的事件
        gameCore.eventSystem.RegistListener(GameEventType.Event_Mine_Gathered, OnMineralMined);
        // 监听请求敌方范围生成的事件
        gameCore.eventSystem.RegistListener(GameEventType.Event_Enemy_Spawn, OnTryEnemySpawn);
    }

    public int returnLimit(float level)
    {
        switch (level)
        {
            case < 15f:
                return 8;
            case < 30f:
                return 14;
            case < 40f:
                return 17;
            case < 50f:
                return 20;
            case < 70f:
                return 25;
            case < 90f:
                return 30;
            default:
                return 35;
        }
    }
    public bool returnProb(float intensity)
    {
        float rand = Random.Range(0, 100f);
        switch (intensity)
        {
            case < 15f:
                return rand < intensity;
            case < 50f:
                return rand < (intensity - 15f) / 20f + 15f;
            case < 90f:
                return rand < (intensity - 35f) / 20f + 35f;
            default:
                return rand < 60f;
        }
    }

    public void OnGameEnd()
    {
        // 得分判定：
        GameScoreInfo scores = new GameScoreInfo();
        //   在限定时间内胜利
        scores.inTime           = (timeTook < GetTimeLimit());
        //   占领目标
        scores.captureObjective = enemyBaseCaptured;
        //   没有阵亡
        scores.noDeath          = (killedDolls < 1);
        //   摧毁全部敌方单位
		scores.allDestroyed     = (killedEnemy >= totalEnemy);
        //   是否失败
        scores.lost             = isLost();
        //   游戏时长
        scores.timeTook         = GetTime();
        //   关卡限定时长
        scores.timeLimit        = GetTimeLimit();
        //   关卡ID
        scores.stageName        = stageName;

        // 跨场景保存数据
        gameCore.sceneMessager.SaveData("game1.scores", scores);
    }

    public void SetTimeTook(float time)
    {
        this.timeTook = time;
        timeCount.text = ((int) time / 60) + ":" + ((int) time % 60).ToString("00");
    }

    public float GetTimeLimit()
    {
        return gameCore.mapCreator.timeLimit;
    }

    public float GetTime()
    {
        return this.timeTook;
    }

    // 敌方单位被击杀时触发
    public void OnEnemyKilled(GameEventData e)
    {
        this.killedEnemy++;
        killCount.text = killedEnemy + "/" + totalEnemy;
    }

    // 获取矿物资源时触发
    public void OnMineralMined(GameEventData e)
    {
        if (gameCore.enemyProb < 100)
        {
            gameCore.addIntensity(1f);
        }
        dangerLevel.text = gameCore.enemyProb.ToString();
    }

    // 该生成敌方单位时触发
    public void OnTryEnemySpawn(GameEventData e)
    {
        // 先过一个概率判定
        //if (!returnProb(gameCore.enemyProb)) { return; }
        // 再过一个上限判定
        if (totalEnemy >= returnLimit(gameCore.enemyProb)) { return; }

        // 然后，生成一个单位
        gameCore.mapCreator.tryNewSpawnEnemy();
    }

    public void FriendlyDead()
    {
        this.killedDolls++;
    }

    public void SpawnDoll()
    {
        this.totalDolls++;
    }

    public void SpawnEnemy()
    {
        this.totalEnemy++;
        killCount.text = killedEnemy + "/" + totalEnemy;
    }

    public void FriendlyBaseLost()
    {
        this.friendlyBaseCaptured = true;
    }

    public void EnemyBaseCaptured()
    {
        this.enemyBaseCaptured = true;
    }

    public bool isLost()
    {
        return friendlyBaseCaptured;
    }

    public void Initialize()
    {
        try
        {
            HUD.SetActive(false);
            killedEnemy = 0;
            totalEnemy = 0;
            killedDolls = 0;
            totalDolls = 0;
            timeTook = 0;
            enemyBaseCaptured = false;
            friendlyBaseCaptured = false;
            killCount.text = "0/0";
            timeCount.text = "0:00";
            if (gameCore.enemyProb < 0)
            {
                dangerLevel.gameObject.SetActive(false);
            }
        } catch
        {
            Debug.LogError(gameObject.name + "抛出了一个错误");
        }
        
    }

    void Update()
    {
        // 更新游戏流逝的时间
        if (gameCore.IsGaming())
		{
            timeSec = (System.DateTime.Now.Ticks - gameCore.timeStart) / 10000000f;
            SetTimeTook(timeSec);
            if (timeSec - timeTick > 5f && gameCore.isRandomGame())
            {
                timeTick = timeSec;
                // 生成一个敌军
                Debug.Log("尝试生成敌军");
                gameCore.eventSystem.TriggerEvent(GameEventType.Event_Enemy_Spawn, new GameEventData(this.gameObject));
            }
        }
    }
}
