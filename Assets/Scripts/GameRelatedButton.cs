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
            Camera.main.gameObject.GetComponent<CallNotification>().showNotification("石油不足哦 \n" +
                "在测试版本里，可以在主页设置增加石油\n " +
                "正式版本中则要手动获取资源");
        }
        
    }
    
}
