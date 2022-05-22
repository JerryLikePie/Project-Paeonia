using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemListButton : MonoBehaviour
{
    public Text textfield;
    public ItemObject item;

    public void setItem(ItemObject item)
    {
        this.item = item;
    }
}
