using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateController : MonoBehaviour, ISaveableObject
{
    public static GameStateController Instance { get; private set; }

    //keeps track of the last room we were in
    int currentRoomID = 0;

    //put other flags to keep track of here, like beat this boss, finished this quest

    bool hasSlingShot;

    public bool HasSlingShot
    {
        get
        {
            return hasSlingShot;
        }
        set
        {
            hasSlingShot = value;
        }
    }

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

    public bool HasGottenIntro { get; set; }

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
        var val = reader.ReadInt();
        HasGottenIntro = val == 1 ? true : false;
        val = reader.ReadInt();
        hasSlingShot = val == 1 ? true : false;
    }

    public void LoadTemp(GameDataReader reader)
    {
        throw new System.NotImplementedException();
    }

    public void SavePerm(GameDataWriter writer)
    {
        writer.WriteInt(currentRoomID);

        if (HasGottenIntro)
        {
            writer.WriteInt(1);
        }
        else
        {
            writer.WriteInt(0);
        }

        if (hasSlingShot)
        {
            writer.WriteInt(1);
        }
        else
        {
            writer.WriteInt(0);
        }
    }

    public void SaveTemp(GameDataWriter writer)
    {
        throw new System.NotImplementedException();
    }
}
