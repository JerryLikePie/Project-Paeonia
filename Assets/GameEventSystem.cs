using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventSystem : MonoBehaviour
{
    public enum EventType
	{
        
	}

    // key: �¼�����
    // value: �¼�������
    private Dictionary<EventType, object> listeners;


    void Start()
    {
        listeners = new Dictionary<EventType, object>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
