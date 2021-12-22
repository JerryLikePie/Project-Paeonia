using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemListButton : MonoBehaviour
{
    public TextMeshProUGUI textfield;
    public ItemObject item;

    public void setItem(ItemObject item)
    {
        this.item = item;
    }
}
