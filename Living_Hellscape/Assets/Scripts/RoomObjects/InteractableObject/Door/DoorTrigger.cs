using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    Door door;
    Collider2D trigger;

    private void Awake()
    {
        door = GetComponentInParent<Door>();
        trigger = GetComponent<Collider2D>();
    }

    public void SignalTrigger()
    {
        door.SignalDoor();
    }

    public void SetToCollider()
    {
        trigger.isTrigger = false;
    }

    public void SetToTrigger()
    {
        trigger.isTrigger = true;
    }
}
