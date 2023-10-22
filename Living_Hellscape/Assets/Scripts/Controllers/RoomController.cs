using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public static RoomController Instance { get; private set; }

    public const float INTER_ROOM_DISTANCE = 6.0f;

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

        if(room == null)
        {
            activeRoomPrefabIndex = -1;
        }
        else
        {
            activeRoomPrefabIndex = activeRoom.ID;
        }
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

    public Room GetRoomByID(int id)
    {
        if (id < 0) return null;

        foreach (Room room in spawnedRooms)
        {
            if (room.ID == id)
            {
                return room;
            }
        }

        return null;
    }

    public bool IsRoomLoaded(int id)
    {
        if (id < 0) return false;

        foreach (Room room in spawnedRooms)
        {
            if(room.ID == id)
            {
                return true;
            }
        }

        return false;
    }

    public void TransitionFloor(FloorTransitionData floorTransitionData)
    {
        //first clear out all of the room data
        SetActiveRoom(null);
        foreach(Room sr in spawnedRooms)
        {
            Destroy(sr.gameObject);
        }
        spawnedRooms.Clear();

        //then load in the new data
        int roomID = floorTransitionData.connectedRoomPrefab.ID;
        Room room = Instantiate(roomPrefabs[roomID]);
        room.OnLoadRoom();
        SetActiveRoom(room);
        room.OnStartEnterRoom();
        room.ConfigureFromPseudoRoomTransitionByConnectionID(floorTransitionData.pseudoRoomConnectionID);
    }


    /*
     * commented out for now maybe we will use in floor transition

    public void RecenterWorld(RoomTransitionData roomTransitionData)
    {
        //cache objects
        Room fromRoom = roomTransitionData.fromRoom;
        Door fromDoor = roomTransitionData.fromDoor;

        Room toRoom = roomTransitionData.toRoom;
        Door toDoor = roomTransitionData.toDoor;

        //get offset from the from room to the door
        Vector2 fromDoorOffset = fromDoor.TargetPos - fromRoom.transform.position;

        //get the distance between doors
        Vector2 doorDifference = toDoor.TargetPos - fromDoor.TargetPos;

        //get offset from the toDoor to the to Room
        Vector2 toDoorOffset = toRoom.transform.position - toDoor.TargetPos;

        //the sum of these is the offfset from 0
        Vector2 totalOffset = fromDoorOffset + doorDifference + toDoorOffset;
        totalOffset = -totalOffset;

        RepositionRooms(totalOffset);
    }

    void RepositionRooms(Vector2 offset)
    {
        Vector3 offset3D = new Vector3(offset.x, offset.y);

        foreach (Room room in spawnedRooms)
        {
            room.transform.position += offset3D;
        }
    }

    */
}
