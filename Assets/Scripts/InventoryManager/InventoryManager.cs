using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public Inventory menuInventory;
    [SerializeField] ItemListButton[] buttonsList;
    [SerializeField] Image displayImage;
    [SerializeField] Text itemTitle;
    [SerializeField] Text itemDescription;
    [SerializeField] Text moneyDisplay;
    [SerializeField] Text oilDisplay;
    [SerializeField] int[] itemsNum;

    /*
     * 1 = 合金
     * 2 = 油
     * 3 = 票
     * 4 = 记忆体
     */

    private void Awake()
    {
        // awake is before start
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
        menuInventory.AddItem(menuInventory.container[id].item, num);
        UpdateInventory();
    }

    public void AddResource(ItemObject item, int num)
    {
        menuInventory.AddItem(item, num);
        UpdateInventory();
    }

    public void DecreaseResource(int id, int num)
    {
        menuInventory.MinusItem(menuInventory.container[id].item, num);
        UpdateInventory();
    }
    public void AddOneResource(int id)
    {
        menuInventory.AddItem(menuInventory.container[id].item, 1);
        UpdateInventory();
    }

    public void DecreaseOneResource(int id)
    {
        menuInventory.MinusItem(menuInventory.container[id].item, 1);
        UpdateInventory();
    }

    public void Decrease500Resource(int id)
    {
        menuInventory.MinusItem(menuInventory.container[id].item, 500);
        UpdateInventory();
    }

    public int InquireResource(string name)
    {
        foreach (InventorySlot i in menuInventory.container)
        {
            Debug.Log("Inquired " + name + ", got " + i.item.itemName);
            if (i.item.itemName == name)
            {
                return i.amount;
            }
        }
        return -1;
    }

    public void LoadinValues()
    {
        try
        {
            string name = "";
            for (int i = 0; i < menuInventory.container.Count; i++)
            {
                name = menuInventory.container[i].item.itemName + "_num";
                itemsNum[i] = PlayerPrefs.GetInt(name, 0);
                menuInventory.container[i].amount = itemsNum[i];
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    public void SaveValues()
    {
        try
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
