using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

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


    public class MyNoise
	{
		System.Random random;

        public MyNoise()
        {
            random = new System.Random();
        }

        public float[,] GetNoiseMap2D(int height, int width)
		{
            float[,] noiseMap = new float[height, width];
            for (int i = 0; i < height; i++)
			{
                for (int j = 0; j < width; j++)
				{
                    noiseMap[i, j] = (float) random.NextDouble();
				}
			}
            return noiseMap;
		}

        public float[,] Perlin2D(int height, int width, float scale)
        {
            float newX, newY;
            float[,] noiseMap = new float[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    newX = i / scale;
                    newY = j / scale;

                    noiseMap[i, j] = Mathf.PerlinNoise(newX , newY) 
                        + 0.5f * Mathf.PerlinNoise(2f * newX, 2f * newY) 
                        + 0.25f * Mathf.PerlinNoise(4f * newX, 4f * newY);
                }
            }
            return noiseMap;
        }

        public static float[,] Threshold(float[,] arr, float thresh)
        {
            int height = arr.GetLength(0);
            int width = arr.GetLength(1);
            float[,] newArr = new float[width, height];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    newArr[i, j] = arr[i, j] > thresh ? 1.0f : 0.0f;
                }
            }
            return newArr;
        }

        // 前 k% 的元素 → 1
        // 其它元素     → 0
        public static float[,] TopPercentageOf(float[,] arr, float percent)
		{
            Debug.Assert(percent >= 0 && percent <= 1);
            float[] flattenArr = arr.Cast<float>().ToArray();
            Array.Sort(flattenArr);
            int thresholdIndex = Mathf.CeilToInt((1-percent) * flattenArr.Length) - 1;
            float threshold = flattenArr[thresholdIndex];
            return Threshold(arr, threshold);
        }


        /// <summary>
        /// 地图块是六边形的
        /// 但生成的地形是平直的二维图像（noise map）
        /// 所以需要根据地图块的中心点位置进行采样（2D noise map → HEX tiles）
        /// </summary>
        /// <param name="noiseMap">生成的地形图（基本上就是噪点图</param>
        /// <param name="boundingBox">在地形图上采样的范围（窗口），根据窗口尺寸可能会有拉伸</param>
        /// <param name="numTilesLie">地图（tiles）的宽度</param>
        /// <param name="numTilesHang">地图（tiles）的高度</param>
        /// <returns></returns>
        /// 
        public float[,] HexSample(float[,] noiseMap, Rect boundingBox, int numTilesHang, int numTilesLie)
        {
            Debug.Assert(numTilesLie > 0 && numTilesHang > 0);
            float[,] tileVals = new float[numTilesLie, numTilesHang];

            // 偶数层横坐标偏移
            float unitXOff = 0.5f;

            float w = noiseMap.GetLength(0);
            float h = noiseMap.GetLength(1);
            Debug.Assert(boundingBox.x >= 0 && boundingBox.y >= 0);
            Debug.Assert(boundingBox.xMax <= w - 1 && boundingBox.yMax <= h - 1);

            float sampleX;  // tile 在 noiseMap 上对应采样点的空间坐标 X
            float sampleY;  // tile 在 noiseMap 上对应采样点的空间坐标 Y
            Vector2Int p00 = new Vector2Int(); 
            float dx = boundingBox.width / (numTilesLie - 1 + unitXOff);
            float dy = boundingBox.height / (numTilesHang - 1);
            float dxOff = dx * unitXOff;

            sampleY = 0f;
            for (int i = 0; i < numTilesHang; i++)
			{
                sampleX = 0;
                for (int j = 0; j < numTilesLie; j++)
				{
                    // 奇数行
                    if (i % 2 == 0)
					{
                        p00.x = Mathf.FloorToInt(sampleX);
                        p00.y = Mathf.FloorToInt(sampleY);
                    }
                    // 偶数行
                    else
                    {
                        p00.x = Mathf.FloorToInt(sampleX + dxOff);
                        p00.y = Mathf.FloorToInt(sampleY);
                    }
                    // 双线性插值
                    float px = sampleX - p00.x;
                    float py = sampleY - p00.y;
                    float s_x0 = Mathf.Lerp(noiseMap[p00.x, p00.y], noiseMap[p00.x + 1, p00.y], px);
                    float s_x1 = Mathf.Lerp(noiseMap[p00.x, p00.y + 1], noiseMap[p00.x + 1, p00.y + 1], px);
                    float s_xy = Mathf.Lerp(s_x0, s_x1, py);
                    // 存入矩阵
                    tileVals[i, j] = s_xy;

                    sampleX += dx;
				}
                sampleY += dy;
			}

            return tileVals;
        }

        /// <summary>
        /// 生成水体
        /// </summary>
        /// <param name="tilesVal">就是把地图高度数据传过来</param>
        /// <param name="lake">如果false，会衍生出合流，true则会只考虑湖泊</param>
        /// <param name="num">地图上要多少个水体</param>
        /// <returns></returns>
        /// 
        public static float[,] GenerateRiver(float[,] tilesVal, bool lake, int num)
        {
            
            int width = tilesVal.GetLength(0);
            int height = tilesVal.GetLength(1);
            float[,] riverMap = new float[width, height];

            // Random starting point

            int safeguard = 0;

            // Generate one or two rivers
            for (int riverCount = 0; riverCount < num; riverCount++)
            {
                int x = UnityEngine.Random.Range(0, width);
                int y = UnityEngine.Random.Range(0, height);

                // River generation loop
                safeguard = 0;
                while (true)
                {
                    // Mark the current position as part of the river
                    riverMap[x, y] = 1;
                    AddRiverWidth(riverMap, x, y);
                    // Check adjacent tiles for lower height
                    Vector2Int nextTile = FindLowerAdjacentTile(tilesVal, x, y);
                    if (nextTile == Vector2Int.zero) // No valid lower tile found
                    {
                        break;
                    }

                    // Move to the next tile
                    x = nextTile.x;
                    y = nextTile.y;
                    safeguard++;

                    if (safeguard > 100)
                    {
                        break;
                    }
                }

                // Generate Upstream
                if (!lake)
                {
                    Vector2Int nextTile = Vector2Int.zero;
                    float dir = UnityEngine.Random.value * 4f;
                    Debug.LogWarning("=Random Direction=: " + dir);
                    safeguard = 0;
                    while (true)
                    {
                        // Mark the current position as part of the river
                        riverMap[x, y] = 1;
                        // Go a certain direction
                        if (dir <= 1)
                        {
                            int[] directions = { 0, 2, 4 };
                            nextTile = FollowDirection(tilesVal, riverMap, x, y, directions);
                        }
                        else if (dir <= 2)
                        {
                            int[] directions = { 0,1, 2, 3 };
                            nextTile = FollowDirection(tilesVal, riverMap, x, y, directions);
                        }
                        else if (dir <= 3)
                        {
                            int[] directions = { 0, 1, 4, 5 };
                            nextTile = FollowDirection(tilesVal, riverMap, x, y, directions);
                        }
                        else
                        {
                            int[] directions = { 1, 3, 5 };
                            nextTile = FollowDirection(tilesVal, riverMap, x, y, directions);
                        }

                        if (nextTile == Vector2Int.zero)
                        {
                            break;
                        }

                        // Move to the next tile
                        x = nextTile.x;
                        y = nextTile.y;
                        safeguard++;

                        if (safeguard > 100)
                        {
                            break;
                        }
                    }
                }
                
            }

            return riverMap;
        }

        private static Vector2Int FindLowerAdjacentTile(float[,] tilesVal, int x, int y)
        {
            int width = tilesVal.GetLength(0);
            int height = tilesVal.GetLength(1);
            float currentHeight = tilesVal[x, y];
            Vector2Int lowestTile = Vector2Int.zero;
            float lowestHeight = currentHeight;

            for (int i = 0; i < 6; i++)//查看左0右1，上左2上右3，下左4下右5，周围的六个格子
            {
                int newX = x + changeX[i];
                int newY;
                if (x % 2 == 0)
                {
                    newY = y + changeZ[i];
                }
                else
                {
                    newY = y + changeZ[i + 6];
                }
                // Check bounds
                if (newX >= 0 && newX < width && newY >= 0 && newY < height)
                {
                    if (tilesVal[newX, newY] < lowestHeight)
                    {
                        lowestHeight = tilesVal[newX, newY];
                        lowestTile = new Vector2Int(newX, newY);
                    }
                }
            }
            return lowestTile;
        }

        private static Vector2Int FollowDirection(float[,] tilesVal, float[,] riverMap, int x, int y, int[] directions)
        {
            int width = tilesVal.GetLength(0);
            int height = tilesVal.GetLength(1);
            float maxChange = 999f;
            float currentHeight = tilesVal[x, y];
            Vector2Int nextTile = Vector2Int.zero;

            foreach (int i in directions)
            {
                int newX = x + changeX[i];
                int newY;
                if (x % 2 == 0)
                {
                    newY = y + changeZ[i];
                }
                else
                {
                    newY = y + changeZ[i + 6];
                }
                // Check bounds
                if (newX >= 0 && newX < width && newY >= 0 && newY < height)
                {
                    if (riverMap[newX, newY] > 0f)
                    {
                        // 如果已经是水体了就继续走
                        nextTile = new Vector2Int(newX, newY);
                    }
                    else if (abs(currentHeight - tilesVal[newX, newY]) < maxChange)
                    {
                        maxChange = abs(currentHeight - tilesVal[newX, newY]);
                        nextTile = new Vector2Int(newX, newY);
                    }
                }
            }
            return nextTile;
        }

        private static void AddRiverWidth(float[,] riverMap, int x, int y)
        {
            int width = riverMap.GetLength(0);
            int height = riverMap.GetLength(1);

            // Width of 2, apply to adjacent tiles
            int[] dx = { 0, 1, -1 };
            int[] dy = { 0, 1, -1 };

            foreach (int i in dx)
            {
                foreach (int j in dy)
                {
                    int newX = x + i;
                    int newY = y + j;

                    // Check bounds and avoid center tile
                    if (newX >= 0 && newX < width && newY >= 0 && newY < height && !(i == 0 && j == 0))
                    {
                        riverMap[newX, newY] = 1;
                    }
                }
            }
        }
    }
}
