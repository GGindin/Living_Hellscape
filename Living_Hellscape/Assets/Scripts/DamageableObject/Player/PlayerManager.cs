using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    [SerializeField]
    PlayerController bodyControllerPrefab, ghostControllerPrefab;

    PlayerController active;

    PlayerController bodyInstance;

    PlayerController ghostInstance;

    bool playerHasControl = false;

    public bool PlayerHasControl => playerHasControl;

    public PlayerInventory Inventory => active.Inventory;

    public PlayerController Active => active;

    public PlayerController BodyInstance => bodyInstance;

    public PlayerController GhostInstance => ghostInstance;

    public Vector2 BodyPosition => bodyInstance.transform.position;
    public Vector2 GhostPosition => ghostInstance.transform.position;

    private void Awake()
    {
        Instance = this;

        InstantiateControllers();
    }

    private void Update()
    {
        if (GameController.Instance.StopUpdates) return;
        if (!playerHasControl) return;
        if (bodyInstance)
        {
            bodyInstance.ControllerUpdate();
        }
        if (ghostInstance)
        {
            ghostInstance.ControllerUpdate();
        }
    }

    private void FixedUpdate()
    {
        if (GameController.Instance.StopUpdates) return;
        if (!playerHasControl) return;
        if (bodyInstance)
        {
            bodyInstance.ControllerFixedUpdate();
        }
        if (ghostInstance)
        {
            ghostInstance.ControllerFixedUpdate();
        }
    }

    public void SetPlayerControl(bool hasControl)
    {
        playerHasControl = hasControl;
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

    public void ParentControllersToManager()
    {
        bodyInstance.transform.SetParent(transform, true);
        ghostInstance.transform.SetParent(transform, true);
    }

    public void FadeInPlayerGhost()
    {
        SwapActiveController();
        var ghost = (GhostPlayerController)ghostInstance;
        StartCoroutine(ghost.ProcessFadeIn());
    }

    public void FadeOutPlayerGhost()
    {
        SwapActiveController();
        var ghost = (GhostPlayerController)ghostInstance;
        StartCoroutine(ghost.ProcessFadeOut());
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
        active.Inventory.UpdateEquipedGear();

        //if we have an active room we need to reset the camera follow
        if (RoomController.Instance && RoomController.Instance.ActiveRoom)
        {
            RoomController.Instance.ActiveRoom.SetupVirtualCamera();
        }
    }
}
