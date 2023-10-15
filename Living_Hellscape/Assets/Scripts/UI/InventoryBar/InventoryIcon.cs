using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryIcon : MonoBehaviour
{
    [SerializeField]
    Button button;

    [SerializeField]
    Image inventoryImage;

    [SerializeField]
    TMP_Text imageText;

    public void SetIcon(Sprite icon)
    {
        if (icon == null)
        {
            inventoryImage.sprite = null;
            inventoryImage.color = Color.clear;
        }
        else
        {
            inventoryImage.sprite = icon;
            inventoryImage.color = Color.white;
        }
    }
}
