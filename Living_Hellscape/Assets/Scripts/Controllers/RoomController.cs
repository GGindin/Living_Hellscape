using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public static RoomController instance;

    [SerializeField]
    Room[] roomPrefabs;

    Room activeRoom;
    int activeRoomPrefabIndex = -1;

    public Room ActiveRoom => activeRoom;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        LoadDefaultRoom();
    }

    public void SetActiveRoom(Room room)
    {
        activeRoom = room;
    }

    public Room LoadRoomByIndex(int index)
    {
        if (index < 0 || index >= roomPrefabs.Length)
        {
            return null;
        }
        else
        {
            return Instantiate(roomPrefabs[index]);
        }
    }

    private void LoadDefaultRoom()
    {
        if(activeRoomPrefabIndex < 0 || activeRoomPrefabIndex >= roomPrefabs.Length)
        {
            activeRoomPrefabIndex = 0;
            activeRoom = Instantiate(roomPrefabs[activeRoomPrefabIndex]);
        }
        else
        {
            activeRoom = Instantiate(roomPrefabs[activeRoomPrefabIndex]);
        }
    }
}
