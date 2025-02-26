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

    // ���ڿ糡�����ݵ����ݰ�
    public struct GameScoreInfo
    {
		public bool inTime;             // ���޶�ʱ����ʤ��
        public bool captureObjective;   // ռ��Ŀ��
        public bool noDeath;            // û������
        public bool allDestroyed;       // �ݻ�ȫ���з���λ
        public bool lost;               // �Ƿ�ʧ��

        public float timeTook;          // ��Ϸʱ��
        public float timeLimit;         // �ؿ��޶�ʱ��

        public string stageName;        // �ؿ�ID
    }

	void Start()
	{
        // �����з���λ����ɱ�¼�
        gameCore.eventSystem.RegistListener(GameEventType.Event_Enemy_Killed, OnEnemyKilled);
	}

	public void OnGameEnd()
    {
        // �÷��ж���
        GameScoreInfo scores = new GameScoreInfo();
        //   ���޶�ʱ����ʤ��
        scores.inTime           = (timeTook < GetTimeLimit());
        //   ռ��Ŀ��
        scores.captureObjective = enemyBaseCaptured;
        //   û������
        scores.noDeath          = (killedDolls < 1);
        //   �ݻ�ȫ���з���λ
		scores.allDestroyed     = (killedEnemy >= totalEnemy);
        //   �Ƿ�ʧ��
        scores.lost             = isLost();
        //   ��Ϸʱ��
        scores.timeTook         = GetTime();
        //   �ؿ��޶�ʱ��
        scores.timeLimit        = GetTimeLimit();
        //   �ؿ�ID
        scores.stageName        = stageName;

        // �糡����������
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

    // �з���λ����ɱʱ����
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
            Debug.LogError(gameObject.name + "�׳���һ������");
        }
        
    }

    void Update()
    {
        // ������Ϸ���ŵ�ʱ��
        if (gameCore.IsGaming())
		{
            SetTimeTook((System.DateTime.Now.Ticks - gameCore.timeStart) / 10000000f);
        }
    }
}
