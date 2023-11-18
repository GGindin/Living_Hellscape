using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class MenuController : MonoBehaviour
{
    protected GameObject currentSelected;

    public GameObject CurrentSelected
    {
        get
        {
            return currentSelected;
        }
        set
        {
            if(currentSelected != value)
            {
                currentSelected = value;
                PlayChangeSelectedSound();
            }
        }
    }

    public void CheckSelected()
    {
        var newCurrent = EventSystem.current.currentSelectedGameObject;

        if (currentSelected != newCurrent)
        {
            CurrentSelected = newCurrent;
        }
    }

    public virtual void PlayButtonDownSound()
    {
        AudioController.Instance.PlaySoundEffect("doorunlock");
    }

    public virtual void PlayChangeSelectedSound()
    {
        AudioController.Instance.PlaySoundEffect("doorunlock");
    }
}
