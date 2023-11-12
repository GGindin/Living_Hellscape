using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NotificationBoxController : MonoBehaviour
{

    public static NotificationBoxController instance { get; set; }
    public TMP_Text currentText;

    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    public void OpenNotificationBox(string newText)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            currentText.text = newText;
        }
    }

    public void CloseNotificationBox() 
    {
        gameObject.SetActive(false);
        currentText.text = "";
    }

    
}
