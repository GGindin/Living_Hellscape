using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    PlayerController playerController;

    public bool PlayerHasControl => playerController.HasControl;

    RoomTransitionData roomTransitionData;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if(playerController && RoomController.instance.ActiveRoom)
        {
            ActivateActiveRoom(RoomController.instance.ActiveRoom);
            enabled = false;
        }
    }

    public void AssignPlayer(PlayerController playerController)
    {
        this.playerController = playerController;

    }

    public void ActivateActiveRoom(Room room)
    {
        room.ConfigureRoom(playerController.transform);
    }

    public void TransitionToRoom(RoomTransitionData transitionData)
    {
        roomTransitionData = transitionData;
        roomTransitionData.toRoom.StartRoomTransition();
        playerController.SetTarget(roomTransitionData.toDoor.TargetPos);
    }

    public void EndRoomTransition()
    {
        roomTransitionData.fromRoom.EndRoomTransition();
        roomTransitionData.fromDoor.OperateDoor();
        roomTransitionData.toDoor.OperateDoor();

        roomTransitionData.fromRoom.UnConfigureActiveRoom(roomTransitionData.toRoom);
        RoomController.instance.SetActiveRoom(roomTransitionData.toRoom);
        roomTransitionData.toRoom.ConfigureRoom(playerController.transform);

        roomTransitionData = new RoomTransitionData();
    }
}
