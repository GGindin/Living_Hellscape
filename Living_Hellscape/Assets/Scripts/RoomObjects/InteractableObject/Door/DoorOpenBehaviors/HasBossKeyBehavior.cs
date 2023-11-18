using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HasBossKeyBehavior", menuName = "DoorOpens/GotItem/BossKey")]
public class HasBossKeyBehavior : DoorOpenBehavior
{
    public override bool ShouldOpenDoor()
    {
        return GameStateController.Instance.HasBossKey;
    }
}
