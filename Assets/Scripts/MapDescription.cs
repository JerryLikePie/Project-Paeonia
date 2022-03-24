using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MapDescription : MonoBehaviour
{
    [SerializeField] private string myName;
    [TextArea] [SerializeField] private string myDescription;
    [SerializeField] private Text thisName;
    [SerializeField] private Text thisDescription;

    public void ChangeDescription()
    {
        thisName.text = myName;
        thisDescription.text = myDescription;
    }
}
