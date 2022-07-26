using UnityEngine;
using System.Collections;

// 工具类，存放各种静态工具方法
public static class Utilities
{
    public static int[] changeX = { 0, 1, 1, 0, -1, -1 };
    public static int[] changeZ = { 1, 1, 0, -1 , 0, 1,
                       1, 0, -1, -1, - 1, 0 };//分奇偶层，当前坐标的“六个可能的下个坐标”

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

    public static float FindDistance(Vector3 x, Vector3 y)
    {
        return Vector3.Distance(x, y);
    }

    public static float FindDistance(GameObject x, Vector3 y)
    {
        return Vector3.Distance(x.transform.position, y);
    }

    public static float FindDistance(Vector3 x, GameObject y)
    {
        return Vector3.Distance(x, y.transform.position);
    }

    // 从地图坐标生成 GameObject 名
    // 例：3, 7 -> "Map3_7" 
    public static string tilePosToName(int row, int col)
	{
        return "Map" + row + '_' + col;
	}


    // 从地图格子 name 获取地图坐标的方法
    // 例："Map10_5" -> Vector(10, 5)
    public static Vector2Int tileNameToPos(string name)
	{
        Vector2Int pos = new Vector2Int();
        name = name.Substring(3);
        string[] ns = name.Split('_');
        pos.x = int.Parse(ns[0]);
        pos.y = int.Parse(ns[1]);
        return pos;
    }

    // 判断两个 Vector3 是否近似相等
    public static bool Vector3Equal(Vector3 a, Vector3 b)
    {
        if (a == null || b == null) return false;
        return Mathf.Abs(a.x - b.x) <= 0.01 && Mathf.Abs(a.y - b.y) <= 0.01 && Mathf.Abs(a.z - b.z) <= 0.01;
    }
}
