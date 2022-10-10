using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameRelatedButton : MonoBehaviour
{
    public string enteringStage;
    public string previousStage;
    private void Start()
    {
        if (previousStage != "none")
        {
            if (PlayerPrefs.GetInt(previousStage, 0) == 0)
            {
                this.gameObject.GetComponent<Button>().interactable = false;
            }
        }
    }
    public void ToGame()
    {
        PlayerPrefs.SetString("Stage_You_Should_Load", enteringStage);
        SceneManager.LoadScene("Game1");
    }
    
}
