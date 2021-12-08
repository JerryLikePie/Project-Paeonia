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
    public void QuitTheGame()
    {
        Application.Quit(0);
    }
    public void resetProgress()
    {
        PlayerPrefs.SetInt("Map_T1-1", 0);
        PlayerPrefs.SetInt("Map_T1-2", 0);
        PlayerPrefs.SetInt("Map_T1-3", 0);
        PlayerPrefs.SetInt("Map_2-1", 0);
        LoadSavedProgress();
    }
    public void LoadSavedProgress()
    {
        try
        {
            int howManyStars = PlayerPrefs.GetInt(enteringStage, 0);
            for (int i = 0; i < howManyStars; i++)
            {
                rating[i].SetActive(true);
            }
            gameObject.SetActive(false);
            if (previousStage == "none")
            {
                gameObject.SetActive(true);
            }
            else
            {
                int previous = PlayerPrefs.GetInt(previousStage, 0);
                if (previous > 1)
                {
                    gameObject.SetActive(true);
                }
            }
        }
        catch
        {

        }
        
    }

}
