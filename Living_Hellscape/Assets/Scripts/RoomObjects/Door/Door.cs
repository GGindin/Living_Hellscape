using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField]
    DoorSection left, right;

    [SerializeField]
    BoxCollider2D trigger;

    [SerializeField]
    GameObject target;

    Room room;

    bool closed = true;

    public Vector3 TargetPos => target.transform.position;

    private void Awake()
    {
        room = GetComponentInParent<Room>();
    }

    private void Start()
    {
        SetDoorSprite();
    }

    public void OperateDoor()
    {
        closed = !closed;
        SetDoorSprite();
    }

    public void SignalDoor()
    {
        if (GameController.instance.PlayerHasControl)
        {
            room.ConfigureRoomTransition(this);
        }
    }

    void SetDoorSprite()
    {
        left.SetDoorSprite(closed);
        right.SetDoorSprite(closed);
    }
}
