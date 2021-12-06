using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Timer
{
    private float moveTime, timeElapsed;
    public float percentageTime,timeLeft;//给外面看的数据
    public float TimeToWait, Buff = 1;//外面输入进来的数据，在这里是不变的
    public long timeStart;//输入的数据
    public bool IsCounting;
    public Unit unit;
    public void TimerUpdate()
    {
        if (IsCounting)    // 如果这时候在CD内
        {
            moveTime = TimeToWait * Buff * 1000 * 10000;
            timeElapsed = System.DateTime.Now.Ticks - timeStart; // 计算已经cd了多久
            percentageTime = timeElapsed / moveTime;
            timeLeft = (moveTime - timeElapsed) / 10000000;
            unit.actionCDBar.value = percentageTime;
            if (percentageTime >= 1f)
            {
                unit.canMove = true;
                IsCounting = false;
            }
        }
    }
}
