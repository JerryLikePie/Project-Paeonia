using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Settings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    private string myName;
    void Start()
    {
        myName = gameObject.name;
        gameObject.GetComponent<Slider>().value = PlayerPrefs.GetFloat(myName, 0.5f);
    }

    public void setVolume(float sliderValue)
    {
        PlayerPrefs.SetFloat(myName, sliderValue);
        audioMixer.SetFloat(myName, Mathf.Log10(sliderValue) * 20);
    }
}
