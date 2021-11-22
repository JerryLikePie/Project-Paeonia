using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public AudioSource bgm;
    public Slider musicVolume;
    // Start is called before the first frame update
    void Start()
    {
        bgm.volume = PlayerPrefs.GetFloat("BGM_volume", 1);
        musicVolume.value = PlayerPrefs.GetFloat("BGM_volume", 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void changeMusicVolume()
    {
        bgm.volume = musicVolume.value;
        PlayerPrefs.SetFloat("BGM_volume", musicVolume.value);
        PlayerPrefs.Save();
    }
}
