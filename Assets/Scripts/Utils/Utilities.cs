using UnityEngine;
using System.Collections;

// 工具类，存放各种静态工具方法
public static class Utilities
{
    public static void SetHealthGradient(Gradient gradient)
    {
        GradientColorKey[] colorKey;
        GradientAlphaKey[] alphaKey;

        colorKey = new GradientColorKey[2];
        colorKey[0].color = Color.red;
        colorKey[0].time = 0.0f;
        colorKey[1].color = Color.green;
        colorKey[1].time = 1.0f;

        alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 1.0f;
        alphaKey[1].time = 1.0f;

        gradient.SetKeys(colorKey, alphaKey);
    }

    public static float FindDistance(GameObject x, GameObject y)
    {
        return Vector3.Distance(x.transform.position, y.transform.position);
    }
    // 判断两个 Vector3 是否近似相等
    public static bool Vector3Equal(Vector3 a, Vector3 b)
    {
        if (a == null || b == null) return false;
        return Mathf.Abs(a.x - b.x) <= 0.01 && Mathf.Abs(a.y - b.y) <= 0.01 && Mathf.Abs(a.z - b.z) <= 0.01;
    }
}
