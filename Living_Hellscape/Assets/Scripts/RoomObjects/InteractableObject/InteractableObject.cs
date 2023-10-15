using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    public abstract Collider2D InteractableCollider { get; }

    public abstract void Interact();

    //the assign methods will be implimented later, they will be used to stop
    //the player from taking an object out of a room
    //make the door trigger a trigger on assign
    //and make the door trigger a collider on unassign
    public abstract void OnAssign();

    public abstract void OnUnassign();
}
