using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ڿ� Scene �������ݵĽű���ͨ��������ֻ֤ÿ�� Scene ʼ����һ������
/// 
/// ��Ҫ�糡����������ʱ��
///   �ڳ�������ǰ���� SaveData() �������ݣ�
///   ���³����л�ȡ SceneMgr ����󣬵��� LoadData() ��ȡ���������
/// 
/// </summary>
public class SceneMessager : MonoBehaviour
{
    // ��ֻ֤��һ�� SceneMgr ��ʵ������
    public static bool hasInstance = false;

    // ���ڱ���糡������
    private Dictionary<string, object> savedData;

    void Awake()
    {
        // ��֤ SceneMgr ʼ�յ�������
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

    // ʹ�ø÷���������Ҫ�糡��������
    // �ظ��������µ����ݻḲ�Ǿɵ�
    public void SaveData(string key, object data)
	{
        Debug.Assert(key != null);
        if (savedData.ContainsKey(key))
        {
            savedData.Remove(key);
        }
        savedData.Add(key, data);
        
	}

    // ɾ�����ݣ�������Ҫ�ж��Ƿ���ڵĳ���
    public void DeleteData(string key)
	{
        if (key != null)
		{
            savedData.Remove(key);
		} 
	}

    // ��ȡ����
    public T LoadData<T>(string key)
    {
        return (T)savedData[key];
    }

    // �����Ƿ����
    public bool HasData(string key)
	{
        return savedData.ContainsKey(key);
	}

}
