using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BetterButtonArea : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.5f;
    }
}
