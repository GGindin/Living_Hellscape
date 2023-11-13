using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateController : MonoBehaviour, ISaveableObject
{
    public static GameStateController Instance { get; private set; }

    //keeps track of the last room we were in
    int currentRoomID = 0;

    //put other flags to keep track of here, like beat this boss, finished this quest

    bool hasGotKnife;
    bool hasSlingShot;
    bool hasGhostWind;
    bool hasGottenIntro;
    bool knowsHowToPossesBody;
    bool beatMiniBoss;

    public bool BeatMiniBoss
    {
        get
        {
            return beatMiniBoss;
        }
        set
        {
            AudioController.Instance.PlaySoundEffect("victory");
            beatMiniBoss = value;
            SaveGameState();
        }
    }

    public bool HasGotKnife
    {
        get
        {
            return hasGotKnife;
        }
        set
        {
            hasGotKnife = value;
            SaveGameState();
        }
    }

    public bool HasSlingShot
    {
        get
        {
            return hasSlingShot;
        }
        set
        {
            AudioController.Instance.PlaySoundEffect("victory");
            hasSlingShot = value;
            SaveGameState();
        }
    }

    public bool HasGhostWind
    {
        get
        {
            return hasGhostWind;
        }
        set
        {
            AudioController.Instance.PlaySoundEffect("victory");
            hasGhostWind = value;
            SaveGameState();
        }
    }

    public bool KnowsHowToPossesBody
    {
        get
        {
            return knowsHowToPossesBody;
        }
        set
        {
            knowsHowToPossesBody = value;
            SaveGameState();
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

    public bool HasGottenIntro
    {
        get
        {

            return hasGottenIntro;
        }
        set
        {
            hasGottenIntro = value;
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
        var val = reader.ReadInt();
        hasGotKnife = val == 1 ? true : false;
        val = reader.ReadInt();
        hasGottenIntro = val == 1 ? true : false;
        val = reader.ReadInt();
        hasSlingShot = val == 1 ? true : false;
        val = reader.ReadInt();
        hasGhostWind = val == 1 ? true : false;
        val = reader.ReadInt();
        knowsHowToPossesBody = val == 1 ? true : false;
        val = reader.ReadInt();
        beatMiniBoss = val == 1 ? true : false;
    }

    public void LoadTemp(GameDataReader reader)
    {
        throw new System.NotImplementedException();
    }

    public void SavePerm(GameDataWriter writer)
    {
        writer.WriteInt(currentRoomID);

        if (HasGotKnife)
        {
            writer.WriteInt(1);
        }
        else
        {
            writer.WriteInt(0);
        }

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

        if (hasGhostWind)
        {
            writer.WriteInt(1);
        }
        else
        {
            writer.WriteInt(0);
        }

        if (knowsHowToPossesBody)
        {
            writer.WriteInt(1);
        }
        else
        {
            writer.WriteInt(0);
        }

        if (beatMiniBoss)
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
