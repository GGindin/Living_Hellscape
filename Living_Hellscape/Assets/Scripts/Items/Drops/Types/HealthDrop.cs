using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDrop : ItemDrop
{
    [SerializeField]
    int healthAmount;

    public override void Collect()
    {
        PlayerManager.Instance.Active.PlayerStats.ChangeCurrentHealth(healthAmount);
        Destroy(gameObject);
    }
}
