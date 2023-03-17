using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitEntity : MonoBehaviour
{
    public int howManyEntity;
    public bool amIShowing = true;
    // Start is called before the first frame update

    bool isFacingRight;

    private void Start()
    {
        isFacingRight = true;
    }

    public void flip(bool turnRight)
    {
        if (isFacingRight && !turnRight)
        {
            isFacingRight = false;
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        else if (!isFacingRight && turnRight)
        {
            isFacingRight = true;
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
