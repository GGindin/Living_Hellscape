using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    Door door;

    private void Awake()
    {
        door = GetComponentInParent<Door>();
    }

    public void SignalTrigger()
    {
        door.SignalDoor();
    }
}
