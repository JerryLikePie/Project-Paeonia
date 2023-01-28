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
    public GameObject starLv1, starLv2;
    public StoryPlay storypanel;
    private void Start()
    {
        updateStatus();
    }
    

    public void updateStatus()
    {
        checkAvailability();
        checkStar();
    }

    private void resetStory()
    {
        PlayerPrefs.DeleteKey(enteringStage);
    }
    private void checkAvailability()
    {
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
                starLv1.SetActive(false);
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
        Debug.Log(inventory.itemsNum[2]);
        if (inventory.itemsNum[2] >= decreaseAmount)
        {
            inventory.DecreaseResource(2, decreaseAmount);
            PlayerPrefs.SetString("Stage_You_Should_Load", enteringStage);
            SceneManager.LoadScene("Game1");
        }
        else
        {
            Camera.main.gameObject.GetComponent<CallNotification>().showNotification("ʯ�Ͳ����޷����� \n" +
                "�ڲ��԰汾��������̵�ֱ�ӻ�ȡʯ��\n " +
                "��ʽ�汾����Ҫ�ֶ���ȡ��Դ");
        }
        
    }

    public void readStory()
    {
        if (storypanel != null)
        {
            storypanel.loadStory(enteringStage);
            PlayerPrefs.SetInt(enteringStage, 3);
        }
        else
        {
            Camera.main.gameObject.GetComponent<CallNotification>().showNotification("������һ�������׳��������\n " +
                gameObject.name + "��" + enteringStage);
        }
    }

}
