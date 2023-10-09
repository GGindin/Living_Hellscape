using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    //PlayerController playerController;

    public PlayerController PlayerController => PlayerManager.instance.Active;

    public bool PlayerHasControl => PlayerController.HasControl;

    RoomTransitionData roomTransitionData;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        LoadInPlayer();
    }

    private void LoadInPlayer()
    {
        var placement = RoomController.instance.ActiveRoom.PlayerSpawnPlacement;
        PlayerController.transform.position = placement.Position;

        RoomController.instance.ActiveRoom.OnStartEnterRoom();
        RoomController.instance.ActiveRoom.OnEnterRoom();
    }

    public void TransitionToRoom(RoomTransitionData transitionData)
    {
        roomTransitionData = transitionData;
        roomTransitionData.toRoom.OnStartEnterRoom();
        PlayerController.SetTarget(roomTransitionData.toDoor.TargetPos);
    }

    public void EndRoomTransition()
    {
        roomTransitionData.toRoom.OnEnterRoom();

        roomTransitionData.fromDoor.OperateDoor();
        roomTransitionData.toDoor.OperateDoor();

        roomTransitionData.fromRoom.OnLeaveRoom();

        //set player to be child of active room
        PlayerController.transform.SetParent(RoomController.instance.ActiveRoom.transform, true);

        //recenter world
        RoomController.instance.RecenterWorld();

        //reset data 
        roomTransitionData = new RoomTransitionData();
    }
}
