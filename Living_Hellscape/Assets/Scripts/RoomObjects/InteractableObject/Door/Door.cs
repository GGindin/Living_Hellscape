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
    DoorTrigger trigger;

    [SerializeField]
    GameObject target;

    [SerializeField]
    bool requiresKey;

    Animator doorAnimator;

    bool isUnlocked;

    CompositeCollider2D compCollider;

    protected Room room;

    bool closed = true;

    int isClosedAnimID = Animator.StringToHash("isClosed");

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
        doorAnimator = GetComponent<Animator>();
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
        if (room.DefeateAllEnemies && room.HasActiveEnemies() && PlayerManager.Instance.PlayerHasControl) return;
        if (closed && requiresKey && !isUnlocked)
        {
            var key = PlayerManager.Instance.Inventory.GetItemByType(typeof(Key));
            if (key)
            {
                key.Activate();
                isUnlocked = true;
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
        if (PlayerManager.Instance.PlayerHasControl)
        {
            room.ConfigureRoomTransition(this);
        }
    }

    protected virtual void SetDoorSprite()
    {
        doorAnimator.SetBool(isClosedAnimID, closed);
    }

    public override string GetFileName()
    {
        //never should get called
        throw new System.NotImplementedException();
    }

    public override void SavePerm(GameDataWriter writer)
    {
        if (requiresKey)
        {
            if (isUnlocked)
            {
                writer.WriteInt(1);
            }
            else
            {
                writer.WriteInt(0);
            }
        }
    }

    public override void LoadPerm(GameDataReader reader)
    {
        if (requiresKey)
        {
            int value = reader.ReadInt();
            if(value == 0)
            {
                isUnlocked = false;
            }
            else
            {
                isUnlocked = true;
            }
        }
    }

    public override void SaveTemp(GameDataWriter writer)
    {
        //never should get called
        throw new System.NotImplementedException();
    }
    public override void LoadTemp(GameDataReader reader)
    {
        //never should get called
        throw new System.NotImplementedException();
    }
}
