using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    /*
     * Main Menu
     * 用于主界面的相关操作
     */
    public StoryPlay storyPanel;
    public GameRelatedButton[] allGameEntries;
    private void Start()
    {
        // 设游戏为60帧
        Application.targetFrameRate = 60;
        // 如果是新版本直接reload
        if (PlayerPrefs.GetFloat("Last_Version", 0.0f) != 0.09f)
        {
            resetProgress();
            PlayerPrefs.SetFloat("Last_Version", 0.09f);
        }
        // 载入存档（这个出正式版之前要改！）
        LoadSavedProgress();
        // 检查开幕剧情
        IntroductionStory();
    }

    void IntroductionStory()
    {
        // 在第一次进入游戏时加载开幕剧情
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
        // 因为怕哪个按钮用了这个函数所以保留。
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
        // 目前除了playerpref之外没有用其他存档手段所以是空的
        // 但是等正式版肯定不能只playerpref
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
