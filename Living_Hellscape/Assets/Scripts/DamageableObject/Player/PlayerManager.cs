using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    [SerializeField]
    PlayerController bodyControllerPrefab, ghostControllerPrefab;

    [SerializeField]
    PlayerInventory inventory;

    PlayerController active;

    PlayerController bodyInstance;

    PlayerController ghostInstance;

    public PlayerInventory Inventory => inventory;

    public PlayerController Active => active;

    public PlayerController BodyInstance => bodyInstance;

    public PlayerController GhostInstance => ghostInstance;

    public Vector2 BodyPosition => bodyInstance.transform.position;
    public Vector2 GhostPosition => ghostInstance.transform.position;

    private void Awake()
    {
        Instance = this;

        InstantiateControllers();

        //temporarily always the body
        SetActiveController(bodyInstance);

        inventory.InstantiateInventory();
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

    public void HandleInventory()
    {
        if (GameController.Instance.Paused)
        {
            InventoryPanelController.Instance.OpenInventory();
        }
        else
        {
            InventoryPanelController.Instance.CloseInventory();
        }
    }

    public void ParentControllersToActiveRoom()
    {
        bodyInstance.transform.SetParent(RoomController.Instance.ActiveRoom.transform, true);
        ghostInstance.transform.SetParent(RoomController.Instance.ActiveRoom.transform, true);
    }

    public void SetControllersPositionToPseduoRoom(PseudoRoom pseudoRoom)
    {
        bodyInstance.transform.position = pseudoRoom.transform.position;
        ghostInstance.transform.position = pseudoRoom.transform.position;
    }

    public void ParentControllersToManager()
    {
        bodyInstance.transform.SetParent(transform, true);
        ghostInstance.transform.SetParent(transform, true);
    }

    public void SetControl(bool control)
    {
        bodyInstance.SetControl(control);
        ghostInstance.SetControl(control);
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

        HealthPanelController.Instance.UpdatePanel(active.PlayerStats);

        //if we have an active room we need to reset the camera follow
        if (RoomController.Instance && RoomController.Instance.ActiveRoom)
        {
            RoomController.Instance.ActiveRoom.SetupVirtualCamera();
        }
    }
}
