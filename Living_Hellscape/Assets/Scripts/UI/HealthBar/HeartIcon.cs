using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartIcon : MonoBehaviour
{
    [SerializeField]
    Color activeColor, inactiveColor;

    [SerializeField]
    Image image;

    bool active;

    public void SetColorState(bool active)
    {
        this.active = active;

        if (active)
        {
            image.color = activeColor;
        }
        else
        {
            image.color = inactiveColor;
        }
    }
}
