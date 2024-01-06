using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirDropdown : MonoBehaviour
{
    /*
     * Air Dropdown
     * 用于对空中单位（敌我双方）生成一个垂直于地面的光标
     * 以便玩家在固定视角下确认空中单位的位置
     */
    public Transform indicator;

    // Update is called once per frame
    void Update()
    {
        if (indicator != null)
        {
            transform.position = Vector3.right * indicator.position.x + Vector3.forward * indicator.position.z + Vector3.up * 0;
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.right * 0.5f + Vector3.forward * 0.5f + Vector3.up * indicator.position.y;
        }
    }
}
