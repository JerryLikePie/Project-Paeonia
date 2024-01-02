using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public StoryPlay storyPanel;
    public GameRelatedButton[] allGameEntries;
    private void Start()
    {
        Application.targetFrameRate = 60;
        LoadSavedProgress();
        IntroductionStory();
    }

    void IntroductionStory()
    {
        Debug.Log(PlayerPrefs.GetInt("Introduction_Done", 0));
        if (PlayerPrefs.GetInt("Introduction_Done", 0) == 0)
        {
            if (storyPanel != null)
            {
                storyPanel.loadStory("Introduction1");
                PlayerPrefs.SetInt("Introduction_Done", 1);
            }
        }
    }

    public void ToGame(string enteringStage)
    {
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
        PlayerPrefs.DeleteKey("Introduction_Done");
        PlayerPrefs.DeleteKey("Stage_You_Should_Load");
        PlayerPrefs.DeleteKey("SD-1");
        PlayerPrefs.DeleteKey("ST-1");
        PlayerPrefs.DeleteKey("TR-1");
        PlayerPrefs.DeleteKey("TR-2");
        PlayerPrefs.DeleteKey("TR-3");
        PlayerPrefs.DeleteKey("Map_1-1");
        PlayerPrefs.DeleteKey("Map_1-2");
    }

    public void skipAllProgress()
    {
        PlayerPrefs.SetInt("Introduction_Done", 1);
        PlayerPrefs.SetInt("ST-1", 4);
        PlayerPrefs.SetInt("TR-1", 4);
        PlayerPrefs.SetInt("TR-2", 4);
        PlayerPrefs.SetInt("TR-3", 4);
        PlayerPrefs.SetInt("Map_1-1", 4);
        PlayerPrefs.SetInt("Map_1-2", 4);
    }

    public void LoadSavedProgress()
    {

    }

    public void updateButtons()
    {
        for (int i = 0; i < allGameEntries.Length; i++)
        {
            allGameEntries[i].updateStatus();
        }
    }

}
