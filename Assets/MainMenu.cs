using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        Application.targetFrameRate = 60;
        LoadSavedProgress();
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
        PlayerPrefs.SetInt("Map_1-1", 0);
        PlayerPrefs.SetInt("Map_1-2", 0);
        PlayerPrefs.SetInt("Map_1-3", 0);
    }
    public void allProgress()
    {

    }
    public void LoadSavedProgress()
    {

    }

}
