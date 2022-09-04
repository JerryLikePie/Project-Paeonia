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
    public Text itemTitle;
    public Text itemDescription;
    public Text moneyDisplay;
    public Text oilDisplay;
    public int[] itemsNum;

    /*
     * 1 = 合金
     * 2 = 油
     * 3 = 票
     * 4 = 记忆体
     */

    private void Awake()
    {
        LoadinValues();
        DontDestroyOnLoad(gameObject);
        for (int i = 0; i < Object.FindObjectsOfType<InventoryManager>().Length; i++)
        {
            if (Object.FindObjectsOfType<InventoryManager>()[i] != this)
            {
                Destroy(Object.FindObjectsOfType<InventoryManager>()[i].gameObject);
            }
        }
        UpdateMenuDisplay();
        
    }

    void OnApplicationQuit()
    {
        for (int i = 0; i < menuInventory.container.Count; i++)
        {
            if (itemsNum[i] != menuInventory.container[i].amount)
            {
                string name = menuInventory.container[i].item.itemName + "_num";
                PlayerPrefs.SetInt(name, menuInventory.container[i].amount);
            }
        }
    }

    public void UpdateMenuDisplay()
    {
        if (moneyDisplay == null || oilDisplay == null)
        {
            return;
        }
        moneyDisplay.text = menuInventory.container[1].amount.ToString();
        oilDisplay.text = menuInventory.container[2].amount.ToString();
    }

    void Update()
    {
        for (int i = 0; i < menuInventory.container.Count; i++)
        {
            if (itemsNum[i] != menuInventory.container[i].amount)
            {
                string name = menuInventory.container[i].item.itemName + "_num";
                itemsNum[i] = menuInventory.container[i].amount;
                PlayerPrefs.SetInt(name, itemsNum[i]);
            }
        }
    }

    public void AddResource(int id, int num)
    {
        menuInventory.AddItem(itemPool[id].item, num);
        UpdateInventory();
    }

    public void DecreaseResource(int id, int num)
    {
        menuInventory.MinusItem(itemPool[id].item, num);
        UpdateInventory();
    }
    public void AddOneResource(int id)
    {
        menuInventory.AddItem(itemPool[id].item, 1);
        UpdateInventory();
    }

    public void DecreaseOneResource(int id)
    {
        menuInventory.MinusItem(itemPool[id].item, 1);
        UpdateInventory();
    }

    public void LoadinValues()
    {
        try
        {
            string name = "";
            for (int i = 0; i < menuInventory.container.Count; i++)
            {
                name = menuInventory.container[i].item.itemName + "_num";
                itemsNum[i] = PlayerPrefs.GetInt(name, 1);
                menuInventory.container[i].amount = itemsNum[i];
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
        }
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
                    wordsToDisplay = menuInventory.container[i].item.itemName + " （" + menuInventory.container[i].amount + "）";
                    buttonsList[j].textfield.text = wordsToDisplay;
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
        itemTitle.text = buttonClicked.item.itemName;
        itemDescription.text = buttonClicked.item.itemDescription;
        displayImage.gameObject.SetActive(true);
        displayImage.sprite = buttonClicked.item.itemSprite;
    }

    public void UponeClearTheItem()
    {
        itemTitle.text = "";
        itemDescription.text = "";
        displayImage.gameObject.SetActive(false);
    }
}
