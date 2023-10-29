using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : InteractableObject
{
    [SerializeField]
    Item itemPrefab;

    [SerializeField]
    BoxCollider2D boxCollider;

    public override Collider2D InteractableCollider => boxCollider;

    public override void Interact()
    {
        throw new System.NotImplementedException();
    }

    public override string GetFileName()
    {
        throw new System.NotImplementedException();
    }

    public override void LoadPerm(GameDataReader reader)
    {
        throw new System.NotImplementedException();
    }

    public override void LoadTemp(GameDataReader reader)
    {
        throw new System.NotImplementedException();
    }

    public override void SavePerm(GameDataWriter writer)
    {
        throw new System.NotImplementedException();
    }

    public override void SaveTemp(GameDataWriter writer)
    {
        throw new System.NotImplementedException();
    }
}
