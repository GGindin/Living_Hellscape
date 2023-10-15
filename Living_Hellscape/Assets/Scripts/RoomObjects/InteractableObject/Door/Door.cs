using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : InteractableObject
{
    [SerializeField]
    DoorSection left, right;

    [SerializeField]
    DoorTrigger trigger;

    [SerializeField]
    GameObject target;

    CompositeCollider2D compCollider;

    Room room;

    bool closed = true;

    public Vector3 TargetPos => target.transform.position;

    public override Collider2D InteractableCollider => compCollider;

    public override void Interact()
    {
        OperateDoor();
    }

    private void Awake()
    {
        room = GetComponentInParent<Room>();
        compCollider = GetComponent<CompositeCollider2D>();
    }

    private void Start()
    {
        SetDoorSprite();
    }

    public void SetTriggerToCollider()
    {
        trigger.SetToCollider();
    }

    public void SetTriggerToTrigger()
    {
        trigger.SetToTrigger();
    }

    public void OperateDoor()
    {
        closed = !closed;
        SetDoorSprite();
    }

    public void SignalDoor()
    {
        if (GameController.Instance.PlayerHasControl)
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
