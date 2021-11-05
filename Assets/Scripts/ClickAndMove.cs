using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickAndMove : MonoBehaviour
{
    Vector3 NextPos;
    public bool TimeToGoUp;
    public bool isUp = false, isDown = false;
    public bool TimeToGoDown;
    public Vector3 WhereIsUp;
    public Vector3 WhereIsDown;
    void Start()
    {
        NextPos = transform.localPosition;
    }
    void Update()
    {
        //transform.Translate(Vector3.up * 30 * Time.deltaTime);
        if (TimeToGoUp)
        {
            NextPos = WhereIsUp;
            TimeToGoUp = false;
        }
        if (TimeToGoDown)
        {
            NextPos = WhereIsDown;
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
        if (Vector3.Distance(transform.localPosition, WhereIsDown) < 1)
            isDown = true;
        else if (Vector3.Distance(transform.localPosition, WhereIsUp) < 1)
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
