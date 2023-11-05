using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Upgrade : Item
{
    public abstract void AddUpgradeToStats(PlayerStats stats);
}
