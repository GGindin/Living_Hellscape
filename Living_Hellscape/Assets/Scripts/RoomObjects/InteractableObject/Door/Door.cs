using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : InteractableObject
{
    //you need to set the connecting rooms to have the same value
    //so generally leave at 0, unless you have multiple doors leading to the same rooms
    //then set each combo to 1 and 1 or 2 and 2 depending on how many connections there are
    [SerializeField]
    int inRoomID;

    //this is the direction the player goes through the door
    [SerializeField]
    DoorDirection direction;

    [SerializeField]
    DoorSection left, right;

    [SerializeField]
    DoorTrigger trigger;

    [SerializeField]
    GameObject target;

    [SerializeField]
    bool requiresKey;

    CompositeCollider2D compCollider;

    protected Room room;

    bool closed = true;

    public Vector3 TargetPos => target.transform.position;

    public override Collider2D InteractableCollider => compCollider;

    public int InRoomID => inRoomID;

    public DoorDirection DoorDirection => direction;

    public override void Interact()
    {
        OperateDoor();
    }

    protected virtual void Awake()
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
        if (room.DefeateAllEnemies && room.HasActiveEnemies() && GameController.Instance.PlayerHasControl) return;
        if (closed && requiresKey)
        {
            var key = PlayerManager.Instance.Inventory.GetItemByType(typeof(Key));
            if (key)
            {
                key.Activate();
            }
            else
            {
                return;
            }
        }
        closed = !closed;
        SetDoorSprite();
    }

    public void OpenDoor()
    {
        if (closed)
        {
            closed = !closed;
            SetDoorSprite();
        }
    }

    public virtual void CloseDoor()
    {
        if(!closed)
        {
            closed = !closed;
            SetDoorSprite();
        }
    }

    public void SignalDoor()
    {
        if (GameController.Instance.PlayerHasControl)
        {
            room.ConfigureRoomTransition(this);
        }
    }

    protected virtual void SetDoorSprite()
    {
        left.SetDoorSprite(closed);
        right.SetDoorSprite(closed);
    }
}
