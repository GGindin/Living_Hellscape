using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreMiniBossNPC : NPC
{
    public override void CheckForTextUpdate()
    {
        Debug.Log(GameStateController.Instance.BeatMiniBoss);
        if (GameStateController.Instance.BeatMiniBoss)
        {
           
            currentText = 1;
        }
        else
        {
            Debug.Log("WTF");
            currentText = 0;
        }
    }

    public override void OnFinishText()
    {

    }
}
