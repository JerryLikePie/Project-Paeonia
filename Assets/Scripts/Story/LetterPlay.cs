using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LetterPlay : MonoBehaviour
{
    Canvas letterPrefab;
    public AudioSource flipSound;
    public Text words;
    public Image photo;

    private void Start()
    {
        letterPrefab = GetComponent<Canvas>();
        letterPrefab.enabled = false;
    }

    public void ShowLetter(string addWord, Sprite addPhoto)
    {
        words.text = addWord;
        photo.sprite = addPhoto;
        letterPrefab.enabled = true;
    }

    public void HideLetter()
    {
        words.text = "placeholder";
        photo.sprite = null;
        letterPrefab.enabled = true;
    }
}
