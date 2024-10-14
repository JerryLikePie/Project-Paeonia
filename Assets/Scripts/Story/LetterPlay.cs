using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;

public class LetterPlay : MonoBehaviour
{
    Canvas letterPrefab;
    public AudioSource flipSound;
    public Text words;
    public Text title;
    public Image photo;
    public Button answer1, answer2;
    public Text answerW1, answerW2;
    int effect1, effect2;

    private void Start()
    {
        letterPrefab = GetComponent<Canvas>();
        letterPrefab.enabled = false;
    }

    public void ShowLetter(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return;
        }
        Letterscene scene = (Letterscene)Resources.Load("Stories/" + name);
        flipSound.Play();
        words.text = scene.words;
        title.text = scene.title;
        photo.sprite = scene.photo;
        letterPrefab.enabled = true;
        PlayerPrefs.SetInt(name, 4);
        answerW1.text = scene.answer;
        effect1 = scene.effectEvent1;
        if (scene.choice)
        {
            answer2.gameObject.SetActive(true);
            answerW2.text = scene.answer2;
            effect2 = scene.effectEvent2;
        }
        else
        {
            answer2.gameObject.SetActive(false);
            effect2 = 0;
        }
    }

    public void HideLetter()
    {
        words.text = "placeholder";
        title.text = "placeholder";
        effect1 = 0;
        effect2 = 0;
        flipSound.Play();
        answer2.gameObject.SetActive(false);
        photo.sprite = null;
        letterPrefab.enabled = false;
    }
}
