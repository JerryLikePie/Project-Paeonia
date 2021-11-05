using UnityEngine;
using System.Collections;

// 工具类，存放各种静态工具方法
public static class Utilities
{
    public static float Find_Distance(GameObject x, GameObject y)
    {
        return Vector3.Distance(x.transform.position, y.transform.position);
    }
}
