using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameRelatedButton : MonoBehaviour
{
    public string enteringStage;
    public string previousStage;
    //public string showUntilFinished;
    public InventoryManager inventory;
    public GameObject starLv1, starLv2;
    public StoryPlay storypanel;
    private void Start()
    {
        // 如果还没完成show until，则该按钮持续显示
        //if (showUntilFinished != null)
        //{
        //    if (PlayerPrefs.GetInt(showUntilFinished, 0) == 0)
        //    {
        //        this.gameObject.SetActive(true);

        //    }
        //}
        updateStatus();
    }
    

    public void updateStatus()
    {
        if (gameObject.activeSelf)
        {
            checkAvailability();
            checkStar();
        }
    }

    public void resetProg()
    {
        PlayerPrefs.DeleteKey(enteringStage);
    }
    public void skipProg()
    {
        PlayerPrefs.SetInt(enteringStage, 4);
    }
    private void checkAvailability()
    {
        // 如果前一关卡不为none而且未完成，则该按钮不可互动
        if (previousStage != "none")
        {
            if (PlayerPrefs.GetInt(previousStage, 0) == 0)
            {
                this.gameObject.GetComponent<Button>().interactable = false;

            } else {
                this.gameObject.GetComponent<Button>().interactable = true;
            }
        } else {
            this.gameObject.GetComponent<Button>().interactable = true;
        }
    }

    public void checkStar()
    {
        if (starLv1 == null || starLv2 == null)
        {
            // 没有的话就不要check了
            return;
        }
        if (enteringStage != "none")
        {
            int num = PlayerPrefs.GetInt(enteringStage, 0);
            //Debug.Log(enteringStage + " status: " + num);
            if (num <= 3 && num > 0)
            {
                starLv1.SetActive(true);
                starLv2.SetActive(false);
            }
            else if (num > 3)
            {
                starLv1.SetActive(true);
                starLv2.SetActive(true);
            }
            else
            {
                starLv1.SetActive(false);
                starLv2.SetActive(false);
            }
        }
    }
    public void ToGame(int decreaseAmount)
    {
        // 进入战斗或教程
        // 在Inspector输入decreaseAmount来扣除一定的油量
        Debug.Log(inventory.itemsNum[2]);
        if (inventory.itemsNum[2] >= decreaseAmount)
        {
            inventory.DecreaseResource(2, decreaseAmount);
            PlayerPrefs.SetString("Stage_You_Should_Load", enteringStage);
            SceneManager.LoadScene("Game1");
        }
        else
        {
            Camera.main.gameObject.GetComponent<CallNotification>().showNotification("石油不足无法出击 \n" +
                "在测试版本里，可以在商店直接获取石油\n " +
                "正式版本中则要手动获取资源");
        }
        
    }

    public void readStory()
    {
        // 用来读剧情的
        // 如果是剧情关卡，就调用这个就可以了
        if (storypanel != null)
        {
            // 把剧情设为已读
            storypanel.loadStory(enteringStage);
            PlayerPrefs.SetInt(enteringStage, 4);
        }
        else
        {
            Camera.main.gameObject.GetComponent<CallNotification>().showNotification("出现了一个错误：抛出错误的是\n " +
                gameObject.name + "的" + enteringStage);
        }
    }

}
