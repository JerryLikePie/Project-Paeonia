using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameRelatedButton : MonoBehaviour
{
    public string enteringStage;
    public string previousStage;
    public InventoryManager inventory;
    private void Start()
    {
        if (previousStage != "none")
        {
            if (PlayerPrefs.GetInt(previousStage, 0) == 0)
            {
                this.gameObject.GetComponent<Button>().interactable = false;
            }
        }
    }
    public void ToGame()
    {
        Debug.Log(inventory.itemsNum[2]);
        if (inventory.itemsNum[2] >= 10)
        {
            inventory.DecreaseResource(2, 10);
            PlayerPrefs.SetString("Stage_You_Should_Load", enteringStage);
            SceneManager.LoadScene("Game1");
        }
        else
        {
            Debug.Log("Ã»ÓÐÓÍµÄÀ²");
        }
        
    }
    
}
