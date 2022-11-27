using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CallNotification : MonoBehaviour
{
    public GameObject notification;
    public Text notifText;
    public Button closeButton;

    public void showNotification(string text)
    {
        notification.gameObject.SetActive(true);
        notifText.text = text;
    }
}
