using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GotGhostWindBehavior", menuName = "DoorOpens/GotItem/GotGhostWind")]
public class GotGhostWindBehavior : DoorOpenBehavior
{
    public override bool ShouldOpenDoor()
    {
        return GameStateController.Instance.HasGhostWind;
    }
}
