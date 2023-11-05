using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour, ISaveableObject
{
    [SerializeField]
    bool ghostObject;

    public bool GhostObject => ghostObject;

    public abstract Collider2D InteractableCollider { get; }

    public abstract SpriteRenderer SpriteRenderer { get; }

    public abstract void Interact();

    public abstract string GetFileName();

    public abstract void LoadPerm(GameDataReader reader);

    public abstract void LoadTemp(GameDataReader reader);

    public abstract void SavePerm(GameDataWriter writer);

    public abstract void SaveTemp(GameDataWriter writer);
}
