using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GotSLingshotBehavior", menuName = "DoorOpens/GotItem/GotSLingShot")]
public class GotSlingShotBehavior : DoorOpenBehavior
{
    public override bool ShouldOpenDoor()
    {
        return GameStateController.Instance.HasSlingShot;
    }
}
