using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 事件类型枚举，新的事件都在这里增加
public enum GameEventType
{
    Event_Enemy_Killed,
    Event_Mine_Gathered,
    Event_Enemy_Spawn
}

// 回调函数的参数类型
public class GameEventData
{
    public GameObject source;
    public string msg = "";
    public object data = null;

    public GameEventData(GameObject source, string msg = "", object data = null)
    {
        this.source = source;
        this.msg = msg;
        this.data = data;
    }

    public GameEventData(GameObject source, object data) : this(source, "", data) { }

}

/// <summary>
/// 事件系统，用于广播游戏中的事件，并触发关心该事件的函数
/// </summary>
public class GameEventSystem : MonoBehaviour
{

    // 回调函数指针
    public delegate void ListenerFunc(GameEventData e);

    // 事件频道
    // key:     事件类型
    // value:   事件触发回调函数
    private Dictionary<GameEventType, ListenerFunc> functionMap;


    // 该脚本先于所有脚本加载，以便其他脚本在 Start 注册监听器的时候可以调用到
    // PS: 其实在声明时初始化也行，但我怕后面改忘了，不如一劳永逸（
    void Start()
    {
        functionMap = new Dictionary<GameEventType, ListenerFunc>();
    }


    public void RegistListener(GameEventType eventType, ListenerFunc func)
	{
        if (functionMap.ContainsKey(eventType))
		{
            functionMap[eventType] += func;
		}
        else
		{
            functionMap.Add(eventType, func);
		}
	}

    public void RemoveListener(GameEventType eventType, ListenerFunc func)
    {
        if (functionMap.ContainsKey(eventType))
        {
            functionMap[eventType] -= func;
            if (functionMap[eventType] == null)
			{
                functionMap.Remove(eventType);
			}
        }
    }
     
    public void TriggerEvent(GameEventType eventType, GameEventData e)
	{
        if (functionMap.ContainsKey(eventType))
		{
            // 通过 delegate 多播触发事件
            ListenerFunc listenerFunc = functionMap[eventType];
            listenerFunc(e);
		}
	}
    
}
