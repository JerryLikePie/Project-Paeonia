using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject[] rating;
    private void Start()
    {
        LoadSavedProgress();
    }
    public void stage2_1()
    {
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
    public void LoadSavedProgress()
    {
        int howManyStars = PlayerPrefs.GetInt("Stars_2_1", 0);
        switch (howManyStars)
        {
            case 1:
                rating[0].SetActive(true);
                break;
            case 2:
                rating[0].SetActive(true);
                rating[1].SetActive(true);
                break;
            case 3:
                rating[0].SetActive(true);
                rating[1].SetActive(true);
                rating[2].SetActive(true);
                break;
            case 4:
                rating[0].SetActive(true);
                rating[1].SetActive(true);
                rating[2].SetActive(true);
                rating[3].SetActive(true);
                break;
            default:
                rating[0].SetActive(false);
                rating[1].SetActive(false);
                rating[2].SetActive(false);
                rating[3].SetActive(false);
                break;
        }
    }

}
