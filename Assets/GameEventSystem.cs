using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventSystem : MonoBehaviour
{
    public enum EventType
	{
        Event_Enemy_Killed
	}


    public class Event
	{
		public GameObject source;
        public string msg = "";
		public object data = null;

        public Event(GameObject source, string msg = "", object data = null)
		{
            this.source = source;
            this.msg = msg;
            this.data = data;
		}

        public Event(GameObject source, object data) : this(source, "", data) { }

    }

    public delegate void ListenerFunc(GameEventSystem.Event e);

    // key: 事件类型
    // value: 事件监听器
    private Dictionary<EventType, ListenerFunc> functionMap;


    void Start()
    {
        functionMap = new Dictionary<EventType, ListenerFunc>();
    }

    public void registListener(EventType eventType, ListenerFunc func)
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

    public void removeListener(EventType eventType, ListenerFunc func)
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
     
    public void triggerEvent(EventType eventType, GameEventSystem.Event e)
	{
        if (functionMap.ContainsKey(eventType))
		{
            // 通过 delegate 多播触发事件
            ListenerFunc listenerFunc = functionMap[eventType];
            listenerFunc(e);
		}
	}
    
}
