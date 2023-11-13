using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreMiniBossNPC : NPC
{
    public override void CheckForTextUpdate()
    {
        if (GameStateController.Instance.BeatMiniBoss)
        {
           
            currentText = 1;
        }
        else
        {
            currentText = 0;
        }
    }

    public override void OnFinishText()
    {

    }
}
