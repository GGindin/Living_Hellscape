using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionIcon : MonoBehaviour
{
    [SerializeField]
    Image image;

    public void SetImage(Sprite icon)
    {
        if(icon != null)
        {
            image.sprite = icon;
            image.color = Color.white;
        }
        else
        {
            image.sprite = null;
            image.color = Color.clear;
        }
    }
}
