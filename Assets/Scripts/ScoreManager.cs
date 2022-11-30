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
    public bool captureObjective, noDeath, allDestroyed, inTime;
    public string stageName;
    public TMPro.TextMeshPro killCount;
    public TMPro.TextMeshPro timeCount;
    public GameObject HUD;
    // Start is called before the first frame update
    public int dropID;
    public int dropAmmount;
    public AudioSource bgmIntense, bgmNormal, bgmIntro;
    public int enemyShown;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        for (int i = 0; i < Object.FindObjectsOfType<ScoreManager>().Length; i++)
        {
            if (Object.FindObjectsOfType<ScoreManager>()[i] != this)
            {
                Destroy(Object.FindObjectsOfType<ScoreManager>()[i].gameObject);
            }
        }
    }


    public void GameEnded()
    {
        if (timeTook < timeLimit)
        {
            inTime = true;
        }
        if (enemyBaseCaptured)
        {
            captureObjective = true;
        }
        if (killedDolls < 1)
        {
            noDeath = true;
        }
        if (killedEnemy >= totalEnemy)
        {
            allDestroyed = true;
        }
    }
    public void SetTime(float time)
    {
        this.timeTook = time;
        timeCount.SetText(((int) time / 60) + ":" + ((int) time % 60).ToString("00"));
    }
    public void SetTimeLimit(float time)
    {
        this.timeLimit = time;
    }
    public float GetTimeLimit()
    {
        return this.timeLimit;
    }
    public float GetTime()
    {
        return this.timeTook;
    }
    public void EnemyKilled()
    {
        this.killedEnemy++;
        killCount.SetText(killedEnemy + "/" + totalEnemy);
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
        killCount.SetText(killedEnemy + "/" + totalEnemy);
    }
    public void FriendlyBaseLost()
    {
        this.friendlyBaseCaptured = true;
    }
    public void EnemyBaseCaptured()
    {
        this.enemyBaseCaptured = true;
    }
    public bool Lost()
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
            killCount.SetText("0/0");
            timeCount.SetText("0:00");
            enemyShown = 0;
        } catch
        {
            Debug.LogError(gameObject.name + "抛出了一个错误");
        }
        
    }
    public void startBGM()
    {
        bgmIntro.Play();
        float introLength = bgmIntro.clip.length;
        if (!bgmIntense.isPlaying)
        {
            bgmIntense.PlayDelayed(introLength);
        }
        if (!bgmNormal.isPlaying)
        {
            bgmNormal.PlayDelayed(introLength);
        }
    }
    public void foundEnemy(int num)
    {
        enemyShown += num;
    }
    private void Update()
    {
        if (bgmIntense != null && bgmNormal != null)
        {
            if (bgmIntense.volume < enemyShown / 3.0f)
            {
                bgmIntense.volume += 0.003f;
            } else if (bgmIntense.volume > enemyShown / 3.0f)
            {
                bgmIntense.volume -= 0.001f;
            }
        }
    }
}
