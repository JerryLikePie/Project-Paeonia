using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ItemType
{
    Currency,
    Equipment,
    Material,
    Food,
    Points,
    Default
}

public abstract class ItemObject : ScriptableObject
{
    public Sprite itemSprite;
    public ItemType itemType;
    public string itemName;
    [TextArea(15, 20)] public string itemDescription;
}
