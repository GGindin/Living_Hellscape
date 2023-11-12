using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bandage : Consumable
{
    public override void Activate()
    {
        PlayerManager.Instance.BodyInstance.PlayerStats.ChangeCurrentHealth(1);
        PlayerManager.Instance.GhostInstance.PlayerStats.ChangeCurrentHealth(1);
        base.Activate();
    }
}
