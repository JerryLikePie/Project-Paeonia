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
    public Text killCount;
    public Text timeCount;
    public GameObject HUD;
    public int dropID;
    public int dropAmmount;

    public GameCore gameCore;

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
            SetTimeTook((System.DateTime.Now.Ticks - gameCore.timeStart) / 10000000f);
        }
    }
}
