using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyDroppable : DroppableObject
{
    public ItemDrop dropPrefab;

    public override bool IsNull => false;

    public ItemDrop GetItemDropInstance()
    {
        return Instantiate(dropPrefab);
    }
}
