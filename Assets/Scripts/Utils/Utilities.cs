using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
using System.Collections.Generic;

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
        colorKey[1].color = new Color(55.0f/255.0f, 125.0f/255.0f, 60.0f/255.0f);
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

    // 判断两个 Vector3 是否近似相等
    public static bool Vector3Equal(Vector3 a, Vector3 b)
    {
        if (a == null || b == null) return false;
        return Mathf.Abs(a.x - b.x) <= 0.01 && Mathf.Abs(a.y - b.y) <= 0.01 && Mathf.Abs(a.z - b.z) <= 0.01;
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

    [System.Serializable]
    public struct DollType
    {
        [SerializeField] public bool tank;
        [SerializeField] public bool spaa;
        [SerializeField] public bool arty;
        [SerializeField] public bool fighter;
        [SerializeField] public bool bomber;
    }

    //一个通用的人物类
    [System.Serializable]
    public class Charactor
    {
        [SerializeField] public string name; //名字
        [SerializeField] public int id; //id
        [SerializeField] public bool isInUse; //是否被占用
        [SerializeField] public DollType type; //是什么
        [SerializeField] public Sprite banner; //图鉴中显示的部分，半身
        [SerializeField] public Sprite avatar; //小图头像，作战中需要用到
        [SerializeField] public DollsProperty stats; //人物的具体数据
        [SerializeField] public GameObject doll; //放在地图上的prefab
    }


    /// <summary>
    /// 反射调用含泛型的方法
    /// 简单来说就是调用 callee.methodName(args)
    /// </summary>
    /// <param name="callee">调用的类名</param>
    /// <param name="methodName">需要调用的方法名</param>
    /// <param name="genericTypes">需要封闭的泛型参数，注意只需要包含泛型参数，且与泛型参数数量和顺序保持一致</param>
    /// <param name="args">方法参数</param>
    public static object invokeTypedMethod(object callee, string methodName, Type[] genericTypes, params object[] args)
	{
        MethodInfo method = callee.GetType().GetMethod(methodName);
        MethodInfo typedMethod = method.MakeGenericMethod(genericTypes);
        return typedMethod.Invoke(callee, args);
    }

    public static T Add<T>(T num1, T num2) where T : struct, IComparable
    {
        if (typeof(T) == typeof(short))
        {
            short a = (short)Convert.ChangeType(num1, typeof(short));
            short b = (short)Convert.ChangeType(num2, typeof(short));
            short c = (short)(a + b);
            return (T)Convert.ChangeType(c, typeof(T));
        }
        else if (typeof(T) == typeof(int))
        {
            int a = (int)Convert.ChangeType(num1, typeof(int));
            int b = (int)Convert.ChangeType(num2, typeof(int));
            int c = a + b;
            return (T)Convert.ChangeType(c, typeof(T));
        }
        else if (typeof(T) == typeof(long))
        {
            long a = (long)Convert.ChangeType(num1, typeof(long));
            long b = (long)Convert.ChangeType(num2, typeof(long));
            long c = a + b;
            return (T)Convert.ChangeType(c, typeof(T));
        }
        else if (typeof(T) == typeof(float))
        {
            float a = (float)Convert.ChangeType(num1, typeof(float));
            float b = (float)Convert.ChangeType(num2, typeof(float));
            float c = a + b;
            return (T)Convert.ChangeType(c, typeof(T));
        }
        else if (typeof(T) == typeof(double))
        {
            double a = (double)Convert.ChangeType(num1, typeof(double));
            double b = (double)Convert.ChangeType(num2, typeof(double));
            double c = a + b;
            return (T)Convert.ChangeType(c, typeof(T));
        }
        else if (typeof(T) == typeof(decimal))
        {
            decimal a = (decimal)Convert.ChangeType(num1, typeof(decimal));
            decimal b = (decimal)Convert.ChangeType(num2, typeof(decimal));
            decimal c = a + b;
            return (T)Convert.ChangeType(c, typeof(T));
        }
        else
        {
            Debug.LogError("Invalid T type for Add: " + typeof(T));
        }

        return default(T);
    }

    public static T Sub<T>(T num1, T num2) where T : struct, IComparable
    {
        if (typeof(T) == typeof(short))
        {
            short a = (short)Convert.ChangeType(num1, typeof(short));
            short b = (short)Convert.ChangeType(num2, typeof(short));
            short c = (short)(a - b);
            return (T)Convert.ChangeType(c, typeof(T));
        }
        else if (typeof(T) == typeof(int))
        {
            int a = (int)Convert.ChangeType(num1, typeof(int));
            int b = (int)Convert.ChangeType(num2, typeof(int));
            int c = a - b;
            return (T)Convert.ChangeType(c, typeof(T));
        }
        else if (typeof(T) == typeof(long))
        {
            long a = (long)Convert.ChangeType(num1, typeof(long));
            long b = (long)Convert.ChangeType(num2, typeof(long));
            long c = a - b;
            return (T)Convert.ChangeType(c, typeof(T));
        }
        else if (typeof(T) == typeof(float))
        {
            float a = (float)Convert.ChangeType(num1, typeof(float));
            float b = (float)Convert.ChangeType(num2, typeof(float));
            float c = a - b;
            return (T)Convert.ChangeType(c, typeof(T));
        }
        else if (typeof(T) == typeof(double))
        {
            double a = (double)Convert.ChangeType(num1, typeof(double));
            double b = (double)Convert.ChangeType(num2, typeof(double));
            double c = a - b;
            return (T)Convert.ChangeType(c, typeof(T));
        }
        else if (typeof(T) == typeof(decimal))
        {
            decimal a = (decimal)Convert.ChangeType(num1, typeof(decimal));
            decimal b = (decimal)Convert.ChangeType(num2, typeof(decimal));
            decimal c = a - b;
            return (T)Convert.ChangeType(c, typeof(T));
        }
        else
        {
            Debug.LogError("Invalid T type for Add: " + typeof(T));
        }

        return default(T);
    }

    public static T Multiply<T>(T num1, T num2) where T : struct, IComparable
    {
        if (typeof(T) == typeof(int))
        {
            int a = (int)Convert.ChangeType(num1, typeof(int));
            int b = (int)Convert.ChangeType(num2, typeof(int));
            int c = a * b;
            return (T)Convert.ChangeType(c, typeof(T));
        }
        else if (typeof(T) == typeof(float))
        {
            float a = (float)Convert.ChangeType(num1, typeof(float));
            float b = (float)Convert.ChangeType(num2, typeof(float));
            float c = a * b;
            return (T)Convert.ChangeType(c, typeof(T));
        }
        else if (typeof(T) == typeof(double))
        {
            double a = (double)Convert.ChangeType(num1, typeof(double));
            double b = (double)Convert.ChangeType(num2, typeof(double));
            double c = a * b;
            return (T)Convert.ChangeType(c, typeof(T));
        }
        else if (typeof(T) == typeof(decimal))
        {
            decimal a = (decimal)Convert.ChangeType(num1, typeof(decimal));
            decimal b = (decimal)Convert.ChangeType(num2, typeof(decimal));
            decimal c = a * b;
            return (T)Convert.ChangeType(c, typeof(T));
        }
        else
        {
            Debug.LogError("Invalid T type for Multiply: " + typeof(T));
        }

        return default(T);
    }

    public static T Divide<T>(T num1, T num2) where T : struct, IComparable
    {
        if (typeof(T) == typeof(int))
        {
            int a = (int)Convert.ChangeType(num1, typeof(int));
            int b = (int)Convert.ChangeType(num2, typeof(int));
            int c = a / b;
            return (T)Convert.ChangeType(c, typeof(T));
        }
        else if (typeof(T) == typeof(float))
        {
            float a = (float)Convert.ChangeType(num1, typeof(float));
            float b = (float)Convert.ChangeType(num2, typeof(float));
            float c = a / b;
            return (T)Convert.ChangeType(c, typeof(T));
        }
        else if (typeof(T) == typeof(double))
        {
            double a = (double)Convert.ChangeType(num1, typeof(double));
            double b = (double)Convert.ChangeType(num2, typeof(double));
            double c = a / b;
            return (T)Convert.ChangeType(c, typeof(T));
        }
        else if (typeof(T) == typeof(decimal))
        {
            decimal a = (decimal)Convert.ChangeType(num1, typeof(decimal));
            decimal b = (decimal)Convert.ChangeType(num2, typeof(decimal));
            decimal c = a / b;
            return (T)Convert.ChangeType(c, typeof(T));
        }
        else
        {
            Debug.LogError("Invalid T type for Multiply: " + typeof(T));
        }

        return default(T);
    }

    public class KeyValueData<K, V>
    {
        public K Key;
        public V Value;
    }

    public static string SerializeDictionaryToJson<K, V> (Dictionary<K, V> dictionary)
    {
        List<KeyValueData<K, V>> dataList = new List<KeyValueData<K, V>>();
        foreach (var kvp in dictionary)
        {
            dataList.Add(new KeyValueData<K, V> { Key = kvp.Key, Value = kvp.Value });
        }
        return JsonUtility.ToJson(dataList.ToArray());
    }

    public static Dictionary<K, V> DeserializeJsonToDictionary<K, V> (string json)
    {
        var dataList = JsonUtility.FromJson<KeyValueData<K, V>[]>(json);
        var dictionary = new Dictionary<K, V>();
        foreach (var data in dataList)
        {
            dictionary[data.Key] = data.Value;
        }
        return dictionary;
    }

    public static float max(float a, float b)
    {
        if (a > b)
        {
            return a;
        }
        return b;
    }

    public static float abs(float a)
    {
        if (a < 0.0f)
        {
            return -a;
        }
        return a;
    }
}
