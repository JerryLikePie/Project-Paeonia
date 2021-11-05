using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int killedEnemy, totalEnemy, killedDolls, totalDolls;
    public bool enemyBaseCaptured = false, friendlyBaseCaptured = false;
    public float finalPoints, killPoints, capturePoints, timePoints, casualtyPoints, timeTook, timeLimit;
    public GameObject enemyList;
    public GameObject friendlyList;
    public bool captureObjective, noDeath, allDestroyed, inTime;
    // Start is called before the first frame update
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
    public void CapturedEnemyPoints()
    {
        capturePoints = 10000;
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
    public void Initialize()
    {
        killedEnemy = 0;
        totalEnemy = 0;
        killedDolls = 0;
        totalDolls = 0;
        timeTook = 0;
        enemyBaseCaptured = false;
        friendlyBaseCaptured = false;
    }
}
