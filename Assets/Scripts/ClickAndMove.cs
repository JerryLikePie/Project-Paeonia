using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickAndMove : MonoBehaviour
{
    Vector3 NextPos;
    public bool TimeToGoUp;
    public bool isUp = false, isDown = false;
    public bool TimeToGoDown;
    public Vector3 hereIsUp;
    public Vector3 hereIsDown;
    [Tooltip("使用加法模式时，默认为up，摁一下变成down")]
    public bool additionMode;
    [Tooltip("使用加法模式时，going down是加上下面这个vector")]
    public Vector3 hereIsChange;
    void Start()
    {
        NextPos = transform.localPosition;
        if (additionMode)
        {
            hereIsUp = NextPos;
            hereIsDown = NextPos + hereIsChange;
        } 
    }
    void Update()
    {
        // TODO 存在反复刷新的问题
        //transform.Translate(Vector3.up * 30 * Time.deltaTime);
        if (TimeToGoUp)
        {
            if (additionMode)
            {
                NextPos = NextPos - hereIsChange;
            }
            else
            {
                NextPos = hereIsUp;
            }
            TimeToGoUp = false;
        }
        if (TimeToGoDown)
        {
            if (additionMode)
            {
                NextPos = NextPos + hereIsChange;
            } else
            {
                NextPos = hereIsDown;
            }
            TimeToGoDown = false;
        }
        transform.localPosition = Vector3.Lerp(transform.localPosition, NextPos, 20.0f * Time.deltaTime);
    }
    public void Going_Up()
    {
        TimeToGoUp = true;
    }
    public void Going_Down()
    {
        TimeToGoDown = true;
    }
    public void ClickToSwitch()
    {
        if (Vector3.Distance(transform.localPosition, hereIsDown) < 1)
            isDown = true;
        else if (Vector3.Distance(transform.localPosition, hereIsUp) < 1)
            isUp = true;
        if (isUp)
        {
            TimeToGoDown = true;
            isUp = false;
        }
        if (isDown)
        {
            TimeToGoUp = true;
            isDown = false;
        }
    }
}
