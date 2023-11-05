using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUpgrade : Upgrade
{
    public override void AddUpgradeToStats(PlayerStats stats)
    {
        stats.ChangeMaxHealth(1);
    }
}
