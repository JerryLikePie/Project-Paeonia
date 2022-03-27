using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject[] rating;
    public string enteringStage;
    public string previousStage;
    private void Start()
    {
        Application.targetFrameRate = 60;
        LoadSavedProgress();
    }
    public void ToGame()
    {
        PlayerPrefs.SetString("Stage_You_Should_Load", enteringStage);
        SceneManager.LoadScene("Game1");
    }
    public void ToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }
    public void ToAboutPage()
    {
        SceneManager.LoadScene("AboutPage");
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

    }
    public void allProgress()
    {

    }
    public void LoadSavedProgress()
    {
    }

}
