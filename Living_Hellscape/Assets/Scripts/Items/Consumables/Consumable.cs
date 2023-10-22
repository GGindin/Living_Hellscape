using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Consumable : Item
{
    public override void Activate()
    {
        UseCount();
        PlayerManager.Instance.Inventory.UpdateInventory();
    }
}
