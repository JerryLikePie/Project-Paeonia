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
    }

    public void HideLetter()
    {
        words.text = "placeholder";
        title.text = "placeholder";
        flipSound.Play();
        photo.sprite = null;
        letterPrefab.enabled = false;
    }
}
