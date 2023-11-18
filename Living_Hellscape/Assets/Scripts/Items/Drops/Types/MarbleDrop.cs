using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarbleDrop : ItemDrop
{
    [SerializeField]
    int ammoAmount;

    public override void Collect()
    {
        base.Collect();
        PlayerManager.Instance.Active.Inventory.AddAmmo(ammoAmount);
        Destroy(gameObject);
    }
}
