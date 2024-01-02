using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventSystem : MonoBehaviour
{
    // �¼�����ö�٣��µ��¼�������������
    public enum EventType
	{
        Event_Enemy_Killed
	}

    // �ص������Ĳ�������
    public class EventData
	{
		public GameObject source;
        public string msg = "";
		public object data = null;

        public EventData(GameObject source, string msg = "", object data = null)
		{
            this.source = source;
            this.msg = msg;
            this.data = data;
		}

        public EventData(GameObject source, object data) : this(source, "", data) { }

    }

    // �ص�����ָ��
    public delegate void ListenerFunc(EventData e);

    // �¼�Ƶ��
    // key:     �¼�����
    // value:   �¼������ص�����
    private Dictionary<EventType, ListenerFunc> functionMap;


    // �ýű��������нű����أ��Ա������ű��� Start ע���������ʱ����Ե��õ�
    // PS: ��ʵ������ʱ��ʼ��Ҳ�У������º�������ˣ�����һ�����ݣ�
    void Start()
    {
        functionMap = new Dictionary<EventType, ListenerFunc>();
    }


    public void RegistListener(EventType eventType, ListenerFunc func)
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

    public void RemoveListener(EventType eventType, ListenerFunc func)
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
     
    public void TriggerEvent(EventType eventType, EventData e)
	{
        if (functionMap.ContainsKey(eventType))
		{
            // ͨ�� delegate �ಥ�����¼�
            ListenerFunc listenerFunc = functionMap[eventType];
            listenerFunc(e);
		}
	}
    
}
