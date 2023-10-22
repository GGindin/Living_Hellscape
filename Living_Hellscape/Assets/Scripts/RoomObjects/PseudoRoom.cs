using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PseudoRoom : Room
{
    [SerializeField]
    PseudoDoor pseudoDoor;

    [SerializeField]
    PseudoRoomConnection pseudoRoomConnection;

    public PseudoRoomConnection PseudoRoomConnection => pseudoRoomConnection;

    public PseudoDoor PseudoDoor => pseudoDoor;

    public override void OnLoadRoom() { }

    public override void OnStartEnterRoom()
    {
        VignetteController.Instance.StartVignette();
    }

    //called when actually finished entering room
    public override void OnEnterRoom()
    {
        //set this, then the update can start the trans
        RoomController.Instance.SetActiveRoom(this);
    }

    //called when player finished transition from room
    //called after the OnEnterRoom
    public override void OnLeaveRoom() { }

    //called when room is destroyed
    public override void OnUnloadRoom() { }


    //called by the door to initiate the room transfer
    public override void ConfigureRoomTransition(Door door)
    {
        FloorTransitionData floorTransitionData = new FloorTransitionData();
        floorTransitionData.pseudoRoomConnectionID = pseudoRoomConnection.pseudoRoomID;
        floorTransitionData.connectedRoomPrefab = pseudoRoomConnection.roomPrefab;

        VignetteController.Instance.EndVignette();
        RoomController.Instance.TransitionFloor(floorTransitionData);
    }
}

[System.Serializable]
public class PseudoRoomConnection
{
    //this is the other actual room that the pseudoroom routes to
    public Room roomPrefab;

    //this is an identifier than the two connecting pseudo rooms match so that the player ends up in the right place
    public int pseudoRoomID;
}

public class FloorTransitionData
{
    public int pseudoRoomConnectionID;
    public Room connectedRoomPrefab;
}
