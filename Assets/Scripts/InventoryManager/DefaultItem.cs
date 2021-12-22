using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Default Object", menuName = "Inventory System/Items/Default")]
public class DefaultItem : ItemObject
{
    public void Awake()
    {
        itemType = ItemType.Default;
    }
}
