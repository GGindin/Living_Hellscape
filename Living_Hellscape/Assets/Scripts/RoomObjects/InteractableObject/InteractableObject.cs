using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    public abstract Collider2D InteractableCollider { get; }

    public abstract void Interact();
}
