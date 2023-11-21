using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossKey : Key
{
    public override void OnFirstAddToInventory()
    {
        if (!GameStateController.Instance.HasBossKey)
        {
            GameStateController.Instance.HasBossKey = true;
        }
    }
}
