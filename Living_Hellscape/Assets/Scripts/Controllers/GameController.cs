using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    PlayerController playerController;

    public PlayerController PlayerController => playerController;

    public bool PlayerHasControl => playerController.HasControl;

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
        playerController = Instantiate(placement.prefab, placement.Position, Quaternion.identity, RoomController.instance.ActiveRoom.transform);

        RoomController.instance.ActiveRoom.OnStartEnterRoom();
        RoomController.instance.ActiveRoom.OnEnterRoom();
    }

    public void TransitionToRoom(RoomTransitionData transitionData)
    {
        roomTransitionData = transitionData;
        roomTransitionData.toRoom.OnStartEnterRoom();
        playerController.SetTarget(roomTransitionData.toDoor.TargetPos);
    }

    public void EndRoomTransition()
    {
        roomTransitionData.toRoom.OnEnterRoom();

        roomTransitionData.fromDoor.OperateDoor();
        roomTransitionData.toDoor.OperateDoor();

        roomTransitionData.fromRoom.OnLeaveRoom();

        //set player to be child of active room
        playerController.transform.SetParent(RoomController.instance.ActiveRoom.transform, true);

        //recenter world
        RoomController.instance.RecenterWorld();

        //reset data 
        roomTransitionData = new RoomTransitionData();
    }
}
