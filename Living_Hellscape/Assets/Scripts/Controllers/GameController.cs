using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public PlayerController PlayerController => PlayerManager.Instance.Active;

    public bool PlayerHasControl => PlayerController.HasControl;

    public bool Paused => paused;

    bool paused = false;

    RoomTransitionData roomTransitionData;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        LoadInPlayer();
    }

    public void SetPause()
    {
        paused = !paused;

        if (paused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public void HandlePause()
    {
        if (paused)
        {
            //open pause menu
        }
        else
        {
            //close pause menu
        }
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

        roomTransitionData.fromDoor.CloseDoor();
        roomTransitionData.toDoor.CloseDoor();

        roomTransitionData.fromRoom.OnLeaveRoom();

        //set player to be child of active room
        PlayerManager.Instance.ParentControllersToActiveRoom();

        //recenter world, sometimes gets wierd Virtual Cam glitches, and isn't needed
        //RoomController.Instance.RecenterWorld(roomTransitionData);


        //reset data 
        roomTransitionData = new RoomTransitionData();
    }

    private void LoadInPlayer()
    {
        var placement = RoomController.Instance.ActiveRoom.PlayerSpawnPlacement;
        PlayerController.transform.position = placement.Position;
        PlayerManager.Instance.ParentControllersToActiveRoom();

        RoomController.Instance.ActiveRoom.OnStartEnterRoom();
        RoomController.Instance.ActiveRoom.OnEnterRoom();
    }
}