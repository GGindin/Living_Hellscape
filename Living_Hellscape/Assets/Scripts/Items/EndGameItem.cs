using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class EndGameItem : Item
{
    [SerializeField]
    string endGameText;

    public override void OnFirstAddToInventory()
    {
        StartCoroutine(DisplayEndText());
    }

    IEnumerator DisplayEndText()
    {
        TextBoxController.instance.OpenTextBox(endGameText);

        while (true)
        {
            if (!TextBoxController.instance.gameObject.activeInHierarchy)
            {
                break;
            }
            yield return null;
        }

        GameStateController.Instance.CurrentRoomIndex = 0;

        GameController.Instance.RollCredits();
    }
}
