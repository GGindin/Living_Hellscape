using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDrop", menuName = "DroppableObjects/Enemy/EnemyDrop")]
public class EnemyDrop : DroppableObject
{
    public ItemDrop dropPrefab;

    public override bool IsNull => false;

    public ItemDrop GetItemDropInstance()
    {
        return Instantiate(dropPrefab);
    }
}
