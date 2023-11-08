using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MarbleDrop", menuName = "DroppableObjects/Enemy/MarbleDrop")]
public class MarbleDropDroppable : EnemyDroppable
{
    public override void OnPreResult(EventArgs e)
    {
        base.OnPreResult(e);
        if (GameStateController.Instance.HasSlingShot)
        {
            IsEnabled = true;
        }
        else
        {
            IsEnabled = false;
        }
    }
}
