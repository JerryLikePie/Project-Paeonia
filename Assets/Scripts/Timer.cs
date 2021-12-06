using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Timer
{
    private float moveTime, timeElapsed;
    public float percentageTime,timeLeft;//�����濴������
    public float TimeToWait, Buff = 1;//����������������ݣ��������ǲ����
    public long timeStart;//���������
    public bool IsCounting;
    public Unit unit;
    public void TimerUpdate()
    {
        if (IsCounting)    // �����ʱ����CD��
        {
            moveTime = TimeToWait * Buff * 1000 * 10000;
            timeElapsed = System.DateTime.Now.Ticks - timeStart; // �����Ѿ�cd�˶��
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
