using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �¼�����ö�٣��µ��¼�������������
public enum GameEventType
{
    Event_Enemy_Killed,
    Event_Mine_Gathered,
    Event_Enemy_Spawn
}

// �ص������Ĳ�������
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
/// �¼�ϵͳ�����ڹ㲥��Ϸ�е��¼������������ĸ��¼��ĺ���
/// </summary>
public class GameEventSystem : MonoBehaviour
{

    // �ص�����ָ��
    public delegate void ListenerFunc(GameEventData e);

    // �¼�Ƶ��
    // key:     �¼�����
    // value:   �¼������ص�����
    private Dictionary<GameEventType, ListenerFunc> functionMap;


    // �ýű��������нű����أ��Ա������ű��� Start ע���������ʱ����Ե��õ�
    // PS: ��ʵ������ʱ��ʼ��Ҳ�У������º�������ˣ�����һ�����ݣ�
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
            // ͨ�� delegate �ಥ�����¼�
            ListenerFunc listenerFunc = functionMap[eventType];
            listenerFunc(e);
		}
	}
    
}
