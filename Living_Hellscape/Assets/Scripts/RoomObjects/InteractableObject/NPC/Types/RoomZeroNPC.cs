using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomZeroNPC : NPC
{
    public override void CheckForTextUpdate()
    {
        if (currentText == 0 && GameStateController.Instance.KnowsHowToPossesBody)
        {
            currentText++;
        }
    }

    public override void OnFinishText()
    {
        if(currentText == 0)
        {
            GameStateController.Instance.KnowsHowToPossesBody = true;
            currentText++;
        }
    }
}
