using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public static RoomController Instance { get; private set; }

    [SerializeField]
    int activeRoomPrefabIndex = -1;

    [SerializeField]
    Room[] roomPrefabs;

    Room activeRoom;  

    HashSet<Room> spawnedRooms = new HashSet<Room>();

    public Room ActiveRoom => activeRoom;

    private void Awake()
    {
        Instance = this;
        LoadDefaultRoom();
    }

    public void SetActiveRoom(Room room)
    {
        activeRoom = room;
        activeRoomPrefabIndex = activeRoom.ID;
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

        activeRoom.OnLoadRoom();
    }

    public void AddRoom(Room room)
    {
        spawnedRooms.Add(room);
    }

    public void RemoveRoom(Room room)
    {
        spawnedRooms.Remove(room);
    }

    public void RecenterWorld()
    {
        //will use the room list to center around the active room and the active room will be set at (0, 0)
    }
}
