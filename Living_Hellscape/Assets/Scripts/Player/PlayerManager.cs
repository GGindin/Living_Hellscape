using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    [SerializeField]
    PlayerController bodyControllerPrefab, ghostControllerPrefab;

    [SerializeField]
    PlayerInventory inventory;

    PlayerController active;

    PlayerController bodyInstance;

    PlayerController ghostInstance;

    public PlayerController Active => active;

    public Vector2 BodyPosition => bodyInstance.transform.position;
    public Vector2 GhostPosition => ghostInstance.transform.position;

    private void Awake()
    {
        instance = this;

        InstantiateControllers();

        //temporarily always the body
        SetActiveController(bodyInstance);
    }

    public void SetActiveController(PlayerController controller)
    {
        active = controller;
        SwitchActive();
    }

    public void SwapActiveController()
    {
        if (active == bodyInstance)
        {
            active = ghostInstance;
        }
        else
        {
            active = bodyInstance;
        }

        SwitchActive();
    }

    void InstantiateControllers()
    {
        bodyInstance = Instantiate(bodyControllerPrefab);
        ghostInstance = Instantiate(ghostControllerPrefab);
    }

    void SwitchActive()
    {
        if(active == bodyInstance)
        {
            ghostInstance.DeactivateController();
            bodyInstance.ActivateController();
        }
        else
        {
            bodyInstance.DeactivateController();
            ghostInstance.ActivateController();
        }

        //if we have an active room we need to reset the camera follow
        if (RoomController.instance && RoomController.instance.ActiveRoom)
        {
            RoomController.instance.ActiveRoom.SetupVirtualCamera();
        }
    }
}
