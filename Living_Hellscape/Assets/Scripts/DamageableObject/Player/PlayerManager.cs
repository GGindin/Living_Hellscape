using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour, ISaveableObject
{
    public static PlayerManager Instance { get; private set; }

    [SerializeField]
    PlayerController bodyControllerPrefab, ghostControllerPrefab;

    bool isSetup;

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
        if(isSetup && !IsPlayerDead())
        {
            if (bodyInstance)
            {
                bodyInstance.ControllerFixedUpdate();
            }
            if (ghostInstance)
            {
                ghostInstance.ControllerFixedUpdate();
            }
        }
       

    }

    public void Initialize()
    {
        InstantiateControllers();
        SetActiveController(BodyInstance);
        LoadPlayerData();
        isSetup = true;
    }

    public void SavePlayerData()
    {
        GameStorageController.Instance.SavePerm(this);
    }

    public void LoadPlayerData()
    {
        GameStorageController.Instance.LoadPerm(this);
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

    /*
    public void ParentControllersToActiveRoom()
    {
        bodyInstance.transform.SetParent(RoomController.Instance.ActiveRoom.transform, true);
        ghostInstance.transform.SetParent(RoomController.Instance.ActiveRoom.transform, true);
    }
    */

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

    public void FadeInPlayerGhostImmediate()
    {
        SwapActiveController();
        var ghost = (GhostPlayerController)ghostInstance;
        ghost.FadeInImmediate();
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

    bool IsPlayerDead()
    {
        if((bodyInstance == null || ghostInstance == null) && !GameOverMenuController.Instance.gameObject.activeInHierarchy)
        {
            GameController.Instance.SetupGameOver();
            return true;
        }

        return false;
    }

    public string GetFileName()
    {
        return "player";
    }

    public void SavePerm(GameDataWriter writer)
    {
        bodyInstance.SavePerm(writer);
        ghostInstance.SavePerm(writer);
    }

    public void LoadPerm(GameDataReader reader)
    {
        bodyInstance.LoadPerm(reader);
        ghostInstance.LoadPerm(reader);
    }

    public void SaveTemp(GameDataWriter writer)
    {
        throw new System.NotImplementedException();
    }

    public void LoadTemp(GameDataReader reader)
    {
        throw new System.NotImplementedException();
    }
}
