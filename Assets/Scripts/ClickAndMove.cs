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
    void Start()
    {
        NextPos = transform.localPosition;
    }
    void Update()
    {
        //transform.Translate(Vector3.up * 30 * Time.deltaTime);
        if (TimeToGoUp)
        {
            NextPos = hereIsUp;
            TimeToGoUp = false;
        }
        if (TimeToGoDown)
        {
            NextPos = hereIsDown;
            TimeToGoDown = false;
        }
        transform.localPosition = Vector3.Lerp(transform.localPosition, NextPos, 20.0f * Time.deltaTime);
    }
    public void Going_Up()
    {
        Debug.Log("Œ“≥¨");
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
