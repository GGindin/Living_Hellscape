using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    PlayerController playerController;

    public bool PlayerHasControl => playerController.HasControl;

    private void Awake()
    {
        instance = this;
    }

    public void AssignPlayer(PlayerController playerController)
    {
        this.playerController = playerController;
    }

    public void TransitionToRoom(Room from, Room to, Door otherDoor)
    {
        var target = otherDoor.TargetPos;
        playerController.SetTarget(target);
    }
}
