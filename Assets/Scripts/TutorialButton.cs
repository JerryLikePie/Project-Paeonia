using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialButton : MonoBehaviour
{
    [SerializeField] Sprite[] images;
    [SerializeField] GameObject background;
    [SerializeField] Image display;
    int index = -1;

    private void Start()
    {
        changePic();
    }

    public void changePic()
    {
        index++;
        if (index < images.Length && index >= 0)
        {
            display.sprite = images[index];
        } else if (index >= images.Length){
            gameObject.SetActive(false);
        }
    }
}
