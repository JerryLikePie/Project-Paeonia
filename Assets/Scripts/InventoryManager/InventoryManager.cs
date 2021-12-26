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
    }

    public void AddTestSkadi()
    {
        menuInventory.AddItem(itemPool[0].item, 1);
        UpdateInventory();
    }

    public void UpdateInventory()
    {
        try
        {
            string wordsToDisplay = "";
            int j = 0;
            for (int i = 0; i < menuInventory.container.Count; i++)
            {
                if (menuInventory.container[i].amount > 0)
                {
                    buttonsList[j].gameObject.SetActive(true);
                    buttonsList[j].setItem(menuInventory.container[i].item);
                    wordsToDisplay = menuInventory.container[i].item.itemName + "£¨" + menuInventory.container[i].amount + "£©";
                    buttonsList[j].textfield.SetText(wordsToDisplay);
                    j++;
                }
            }
            while (j < buttonsList.Length)
            {
                buttonsList[j].gameObject.SetActive(false);
                j++;
            }
        } catch(System.Exception ex)
        {
            Debug.LogError(ex);
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
