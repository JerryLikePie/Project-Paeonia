using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MapDescription : MonoBehaviour
{
    [SerializeField] private string myName;
    [SerializeField] private string areaIn;
    [TextArea] [SerializeField] private string myDescription;
    [SerializeField] public int livePercent;
    [SerializeField] public int airDom;

    [SerializeField] private Text thisName;
    [SerializeField] private Text thisDescription;
    [SerializeField] private Text areaName;
    [SerializeField] private Text areaDescription;

    public void ChangeDescription()
    {
        thisName.text = myName;
        thisDescription.text = myDescription;
        areaName.text = areaIn;
        areaDescription.text = "活性化："+livePercent+"%\n制空权："+airDom+"% ";
    }
}
