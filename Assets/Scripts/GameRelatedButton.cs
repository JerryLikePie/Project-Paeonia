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
        // �����û���show until����ð�ť������ʾ
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
        // ���ǰһ�ؿ���Ϊnone����δ��ɣ���ð�ť���ɻ���
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
            // û�еĻ��Ͳ�Ҫcheck��
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
        // ����ս����̳�
        // ��Inspector����decreaseAmount���۳�һ��������
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
        // �����������
        // ����Ǿ���ؿ����͵�������Ϳ�����
        if (storypanel != null)
        {
            // �Ѿ�����Ϊ�Ѷ�
            storypanel.loadStory(enteringStage);
            PlayerPrefs.SetInt(enteringStage, 4);
        }
        else
        {
            Camera.main.gameObject.GetComponent<CallNotification>().showNotification("������һ�������׳��������\n " +
                gameObject.name + "��" + enteringStage);
        }
    }

}
