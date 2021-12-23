using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public Item[] itemPool;
    public Inventory menuInventory;
    public ItemListButton[] buttonsList;
    public Image displayImage;
    public TextMeshPro itemTitle;
    public TextMeshPro itemDescription;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        for (int i = 0; i < Object.FindObjectsOfType<InventoryManager>().Length; i++)
        {
            if (Object.FindObjectsOfType<InventoryManager>()[i] != this)
            {
                Destroy(Object.FindObjectsOfType<InventoryManager>()[i].gameObject);
            }
        }
    }

    void Start()
    {
        UpdateInventory();
    }

    public void AddTestSkadi()
    {
        menuInventory.AddItem(itemPool[0].item, 1);
        UpdateInventory();
    }

    public void UpdateInventory()
    {
        string wordsToDisplay = "";
        int j = 0;
        for (int i = 0; i < buttonsList.Length; i++) //查看Inventory里面有多少Item
        {
            if (j < menuInventory.container.Count) //只要不超过Inventory的大小
            {
                while (menuInventory.container[j].amount == 0)//如果这个Item的数量是0
                {
                    j++; //那就跳过，一直到下一个不是0的Item
                }
                if (j < menuInventory.container.Count)//再确保一遍
                {
                    buttonsList[i].gameObject.SetActive(true);
                    buttonsList[i].setItem(menuInventory.container[j].item);
                    wordsToDisplay = menuInventory.container[j].item.itemName + "（" + menuInventory.container[j].amount + "）";
                    buttonsList[i].textfield.SetText(wordsToDisplay);
                    j++;
                }
                else
                {
                    buttonsList[i].gameObject.SetActive(false);
                }
            }
            else
            {
                buttonsList[i].gameObject.SetActive(false);
            }
        }
    }

    public void UponItemClicked(ItemListButton buttonClicked)
    {
        itemTitle.SetText(buttonClicked.item.itemName);
        itemDescription.SetText(buttonClicked.item.itemDescription);
        displayImage.gameObject.SetActive(true);
        displayImage.sprite = buttonClicked.item.itemSprite;
    }

    public void UponeClearTheItem()
    {
        itemTitle.SetText("");
        itemDescription.SetText("");
        displayImage.gameObject.SetActive(false);
    }
}
