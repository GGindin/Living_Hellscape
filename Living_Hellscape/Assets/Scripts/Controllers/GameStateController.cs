using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateController : MonoBehaviour, ISaveableObject
{
    public static GameStateController Instance { get; private set; }

    //keeps track of the last room we were in
    int currentRoomID = 0;

    //put other flags to keep track of here, like beat this boss, finished this quest

    public int CurrentRoomIndex
    {
        get
        {
            return currentRoomID;
        }
        set
        {
            currentRoomID = value;
            SaveGameState();
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    public void LoadGameState()
    {
        GameStorageController.Instance.LoadPerm(this);
    }

    public void SaveGameState()
    {
        GameStorageController.Instance.SavePerm(this);
    }

    public string GetFileName()
    {
        return "GameState";
    }

    public void LoadPerm(GameDataReader reader)
    {
        if (reader == null) return;

        var roomIndex = reader.ReadInt();
        currentRoomID = roomIndex;
    }

    public void LoadTemp(GameDataReader reader)
    {
        throw new System.NotImplementedException();
    }

    public void SavePerm(GameDataWriter writer)
    {
        writer.WriteInt(currentRoomID);
    }

    public void SaveTemp(GameDataWriter writer)
    {
        throw new System.NotImplementedException();
    }
}
