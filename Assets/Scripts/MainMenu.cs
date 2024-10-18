using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    /*
     * Main Menu
     * �������������ز���
     */
    public StoryPlay storyPanel;
    public GameRelatedButton[] allGameEntries;
    private void Start()
    {
        // ����ϷΪ60֡
        Application.targetFrameRate = 60;
        // ������°汾ֱ��reload
        if (PlayerPrefs.GetFloat("Last_Version", 0.0f) != 0.09f)
        {
            resetProgress();
            PlayerPrefs.SetFloat("Last_Version", 0.09f);
        }
        // ����浵���������ʽ��֮ǰҪ�ģ���
        LoadSavedProgress();
        // ��鿪Ļ����
        IntroductionStory();
    }

    void IntroductionStory()
    {
        // �ڵ�һ�ν�����Ϸʱ���ؿ�Ļ����
        //Debug.Log(PlayerPrefs.GetInt("Introduction1_Done", 0));
        if (PlayerPrefs.GetInt("Introduction1_Done", 0) == 0)
        {
            if (storyPanel != null)
            {
                storyPanel.loadStory("Introduction1");
                PlayerPrefs.SetInt("Introduction1_Done", 1);
            }
        }
        else if (PlayerPrefs.GetInt("Introduction2_Done", 0) == 0)
        {
            if (storyPanel != null)
            {
                storyPanel.loadStory("Introduction2");
                PlayerPrefs.SetInt("Introduction2_Done", 1);
            }
        }
        else if (PlayerPrefs.GetInt("Introduction3_Done", 0) == 0)
        {
            if (storyPanel != null)
            {
                storyPanel.loadStory("Introduction3");
                PlayerPrefs.SetInt("Introduction3_Done", 1);
            }
        }
        else if (PlayerPrefs.GetInt("Introduction4_Done", 0) == 0)
        {
            if (storyPanel != null)
            {
                storyPanel.loadStory("Introduction4");
                PlayerPrefs.SetInt("Introduction4_Done", 1);
            }
        }
        else if (PlayerPrefs.GetInt("Introduction5_Done", 0) == 0)
        {
            if (storyPanel != null)
            {
                storyPanel.loadStory("Introduction5");
                PlayerPrefs.SetInt("Introduction5_Done", 1);
            }
        }
    }

    public void ToGame(string enteringStage)
    {
        // the function that loads into a stage
        // legacy function, now uses GameRelatedButton->ToGame().
        // ��Ϊ���ĸ���ť��������������Ա�����
        PlayerPrefs.SetString("Stage_You_Should_Load", enteringStage);
        SceneManager.LoadScene("Game1");
    }
    public void ToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }
    public void testScene()
    {
        SceneManager.LoadScene("DramaScene");
    }
    public void QuitTheGame()
    {
        Application.Quit(0);
    }
    public void resetProgress()
    {
        // function that's used to clear progress. remember to update key if added.
        PlayerPrefs.DeleteKey("Introduction1_Done");
        PlayerPrefs.DeleteKey("Introduction2_Done");
        PlayerPrefs.DeleteKey("Introduction3_Done");
        PlayerPrefs.DeleteKey("Introduction4_Done");
        PlayerPrefs.DeleteKey("Introduction5_Done");
        PlayerPrefs.DeleteKey("Stage_You_Should_Load");
        iterateResetStages();
    }

    public void skipAllProgress()
    {
        // function that's used to complete all progress. remember to update key if added.
        PlayerPrefs.SetInt("Introduction1_Done", 1);
        PlayerPrefs.SetInt("Introduction2_Done", 1);
        PlayerPrefs.SetInt("Introduction3_Done", 1);
        PlayerPrefs.SetInt("Introduction4_Done", 1);
        PlayerPrefs.SetInt("Introduction5_Done", 1);
        iterateSkipStages();
    }

    public void LoadSavedProgress()
    {
        // Ŀǰ����playerpref֮��û���������浵�ֶ������ǿյ�
        // ���ǵ���ʽ��϶�����ֻplayerpref
    }

    public void updateButtons()
    {
        for (int i = 0; i < allGameEntries.Length; i++)
        {
            allGameEntries[i].updateStatus();
        }
    }

    private void iterateResetStages()
    {
        for (int i = 0; i < allGameEntries.Length; i++)
        {
            allGameEntries[i].resetProg();
        }
    }
    private void iterateSkipStages()
    {
        for (int i = 0; i < allGameEntries.Length; i++)
        {
            allGameEntries[i].skipProg();
        }
    }

}
