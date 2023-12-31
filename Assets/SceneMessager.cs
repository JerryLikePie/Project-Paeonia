using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用于跨 Scene 传递数据的脚本，通过单例保证只每个 Scene 始终有一个对象
/// 
/// 需要跨场景传递数据时：
///   在场景结束前调用 SaveData() 保存数据，
///   在新场景中获取 SceneMgr 对象后，调用 LoadData() 读取保存的数据
/// 
/// </summary>
public class SceneMessager : MonoBehaviour
{
    // 保证只有一个 SceneMgr 的实例存在
    public static bool hasInstance = false;

    // 用于保存跨场景数据
    private Dictionary<string, object> savedData;

    void Awake()
    {
        // 保证 SceneMgr 始终单例运行
        if (!hasInstance)
        {
            DontDestroyOnLoad(this.gameObject);
            hasInstance = true;
            savedData = new Dictionary<string, object>();
        }
        else
		{
            Destroy(this.gameObject);
		}
    }

    // 使用该方法保存需要跨场景的数据
    // 重复调用则新的数据会覆盖旧的
    public void SaveData(string key, object data)
	{
        Debug.Assert(key != null);
        if (savedData.ContainsKey(key))
        {
            savedData.Remove(key);
        }
        savedData.Add(key, data);
        
	}

    // 删除数据，用于需要判断是否存在的场合
    public void DeleteData(string key)
	{
        if (key != null)
		{
            savedData.Remove(key);
		} 
	}

    // 读取数据
    public T LoadData<T>(string key)
    {
        return (T)savedData[key];
    }

    // 数据是否存在
    public bool HasData(string key)
	{
        return savedData.ContainsKey(key);
	}

}
