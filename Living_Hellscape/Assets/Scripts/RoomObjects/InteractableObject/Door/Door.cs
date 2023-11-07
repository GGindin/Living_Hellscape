using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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

    public override SpriteRenderer SpriteRenderer => null;

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
                TextBoxController.instance.OpenTextBox("You hear the lock click. You turn the knob and the door swings open!");
                isUnlocked = true;
            }
            else
            {
                TextBoxController.instance.OpenTextBox("This door is locked. Maybe you can find a key somewhere?");
                return;
            }
        }
        TextBoxController.instance.OpenTextBox("You just opened a door! Press the main action button (currently J) to continue. Pressing this button while the text is being written will allow you to skip to the end of that line instantly. I am going to continue writing here so you can see how the text box works. Feel free to make any suggestions you want and I'll see if I can add them. I currently plan to add a way to control where the line breaks happen (optionally) and a cursor that follows the text (if possible). Maxchars seems a bit janky and I'd like to add a different way to tell when a line ends without manually checking for every line of dialogue and setting it up. I hope it makes sense how you can add text to an object. Thank you!");
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

    public float GetDistanceToRoomEdge(Tilemap intTileMap, Tilemap extTileMap)
    {
        var intTargetPos = intTileMap.WorldToCell(target.transform.position);


        bool interior = true;

        var currentPos = intTargetPos;
        var dir = direction.DirectionToVector2();

        while (true)
        {
            currentPos += new Vector3Int((int)dir.x, (int)dir.y, 0);
            if (interior)
            {
                if (intTileMap.GetTile(currentPos) == null)
                {
                    interior = false;
                }
            }
            else
            {
                if (extTileMap.GetTile(currentPos) == null)
                {
                    break;
                }
            }
        }

        //because of where the origin is going in south or west direction takes 1 extra to find null
        if(direction == DoorDirection.South || direction == DoorDirection.West)
        {
            currentPos -= new Vector3Int((int)dir.x, (int)dir.y, 0);
        }

        var cellDistance = (currentPos - intTargetPos);
        var worldDistance = new Vector3(cellDistance.x * intTileMap.cellSize.x, cellDistance.y * intTileMap.cellSize.y).magnitude;

        var intCellWorldPos = intTileMap.CellToWorld(intTargetPos);
        var targetOffsetFromCell = target.transform.position - intCellWorldPos;

        switch (direction)
        {
            case DoorDirection.North:
                worldDistance -= targetOffsetFromCell.y;
                break;
            case DoorDirection.South:
                worldDistance += targetOffsetFromCell.y;
                break;
            case DoorDirection.West:
                worldDistance += targetOffsetFromCell.x;
                break;
            case DoorDirection.East:
                worldDistance -= targetOffsetFromCell.x;
                break;
            default: break;
        }

        return worldDistance;
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
