using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public PlayerController PlayerController => PlayerManager.Instance.Active;

    bool stopUpdates = false;

    public bool StopUpdates => stopUpdates;

    public bool Paused => paused;

    bool paused = false;

    RoomTransitionData roomTransitionData;

    private void Awake()
    {
        Instance = this;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Start()
    {
        if (!SceneController.Instance)
        {
            StartPlaySession(-1);
        }
    }

    public void StartPlaySession(int saveFile)
    {
        //tell all controllers to startup and load data
        GameStorageController.Instance.Initialize(saveFile);
        GameStateController.Instance.LoadGameState();
        RoomController.Instance.Initialize();
        PlayerManager.Instance.Initialize();
        SetPause(false);
        HandlePause();
        LoadInPlayer();
    }

    public void EndPlaySession()
    {
        SaveGame();
        //tell all controllers to save data
        //then init scene change through scene controller to go to main menu
        //for now we just quit or swap scenes
        SetPause(false);
        stopUpdates = true;

        if (!SceneController.Instance)
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        else
        {
            SceneController.Instance.LoadMainMenuScene();
        }

    }

    public void ReloadPlaySession()
    {
        SaveGame();
        //tell all controllers to save data
        //then init scene change through scene controller to go to main menu
        //for now we just quit or swap scenes
        SetPause(false);
        stopUpdates = true;

        if (!SceneController.Instance)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }
        else
        {
            SceneController.Instance.ReloadPlayerSessionScene();
        }

    }

    public void SetupGameOver()
    {
        GameOverMenuController.Instance.OpenGameOverMenu();
    }

    public void SetPause(bool isPaused)
    {
        paused = isPaused;

        if (paused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public void SwitchPause()
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
            PauseMenuController.Instance.OpenPauseMenu();
        }
        else
        {
            //close pause menu
            PauseMenuController.Instance.ClosePauseMenu();
        }
    }

    public void SetStopUpdates(bool stop)
    {
        stopUpdates = stop;
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


        if(RoomController.Instance.ActiveRoom is PseudoRoom)
        {
            //reset data 
            roomTransitionData = new RoomTransitionData();

            PlayerManager.Instance.SetPlayerControl(false);
            //PlayerManager.Instance.ParentControllersToManager();

            //doesn't actually need a door because it is a pseudo room
            //this just starts another transition to the next floor
            RoomController.Instance.ActiveRoom.ConfigureRoomTransition(null);
        }
        else
        {        
            //set player to be child of active room
            //PlayerManager.Instance.ParentControllersToActiveRoom();

            //recenter world, sometimes gets wierd Virtual Cam glitches, and isn't needed
            //if we do this it might be better to do it before the transition and signal to CM then when only one camera is acting
            //RoomController.Instance.RecenterWorld(roomTransitionData);

            EnemyGhostManager.Instance.RepositionGhosts();

            PlayerManager.Instance.SavePlayerData();

            //reset data 
            roomTransitionData = new RoomTransitionData();
        }
    }

    public void SwitchWorlds()
    {
        //going into ghost world
        if(PlayerManager.Instance.Active != PlayerManager.Instance.GhostInstance)
        {
            PlayerManager.Instance.FadeInPlayerGhost();
            EnemyGhostManager.Instance.StartFadeIn();
            RoomController.Instance.StartFadeIn(); //not implimented yet
            GhostWorldFilterController.Instance.StartFilter();
        }
        //going into human world
        else
        {
            PlayerManager.Instance.FadeOutPlayerGhost();
            EnemyGhostManager.Instance.StartFadeOut();
            RoomController.Instance.StartFadeOut();//not implimented yet
            GhostWorldFilterController.Instance.EndFilter();
        }
    }

    public void GoToGhostNow()
    {
        PlayerManager.Instance.FadeInPlayerGhostImmediate();
        EnemyGhostManager.Instance.DestroyAllGhosts();
        RoomController.Instance.FadeInImmediate();
        GhostWorldFilterController.Instance.SetFilterFull();
    }

    public void GoToBodyNow()
    {
        PlayerManager.Instance.FadeOutPlayerGhostImmediate();
        EnemyGhostManager.Instance.DestroyAllGhosts();
        RoomController.Instance.FadeOutImmediate();
        GhostWorldFilterController.Instance.SetFilterNone();
    }

    void SaveGame()
    {
        GameStateController.Instance.SaveGameState();
        PlayerManager.Instance.SavePlayerData();
        RoomController.Instance.SaveRoomData();      
    }

    private void LoadInPlayer()
    {
        var placement = RoomController.Instance.ActiveRoom.PlayerSpawnPlacement;
        PlayerController.transform.position = placement.Position;
        //PlayerManager.Instance.ParentControllersToActiveRoom();
        PlayerManager.Instance.ParentControllersToManager();

        RoomController.Instance.ActiveRoom.OnStartEnterRoom();
        RoomController.Instance.ActiveRoom.OnEnterRoom();

        PlayerManager.Instance.SetPlayerControl(true);

        if(!GameStateController.Instance.HasGottenIntro && GameStateController.Instance.CurrentRoomIndex == 0)
        {
            ScriptingController.Instance.RunIntro();
        }
        else
        {
            GoToBodyNow();
        }
    }


}
