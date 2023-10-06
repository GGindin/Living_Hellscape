using Cinemachine;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Room : MonoBehaviour
{
    [SerializeField]
    int id;

    [SerializeField]
    RoomPrefabConnection[] prefabConnections;

    RoomConnection[] connections;

    [SerializeField]
    CinemachineVirtualCamera virtualCamera;

    private void Awake()
    {
        connections = new RoomConnection[prefabConnections.Length];
    }

    //called when loaded
    public void ConfigureRoom(Transform cameraFollow)
    {
        for(int i = 0; i < prefabConnections.Length; i++)
        {
            //if already has this setup from a previous load skip
            if (connections[i] != null && connections[i].otherRoom != null) continue;

            //otherwise setup the other room and connections to this room
            Room room = RoomController.instance.LoadRoomByIndex(prefabConnections[i].otherRoomPrefabID);
            room.virtualCamera.Follow = cameraFollow;
            connections[i] = new RoomConnection()
            {
                otherRoom = room,
                thisRoomDoor = prefabConnections[i].thisRoomDoor
            };
            room.SetupRoomConnections();
        }

        virtualCamera.Follow = cameraFollow;
        virtualCamera.gameObject.SetActive(true);
    }

    public void UnConfigureActiveRoom(Room nextActiveRoom)
    {
        for(int i = 0; i < connections.Length; i++)
        {
            var connection = connections[i];
            if(connection.otherRoom != nextActiveRoom)
            {
                Destroy(connection.otherRoom.gameObject);
            }
        }
    }

    void SetupRoomConnections()
    {
        var activeRoom = RoomController.instance.ActiveRoom;
        for(int i = 0; i < prefabConnections.Length; i++)
        {
            var prefab = prefabConnections[i];
            if(prefab.otherRoomPrefabID == activeRoom.id)
            {
                connections[i] = new RoomConnection()
                {
                    otherRoom = activeRoom,
                    thisRoomDoor = prefab.thisRoomDoor
                };
            }
        }
    }

    //called on the to room
    public void StartRoomTransition()
    {
        virtualCamera.gameObject.SetActive(true);
    }

    //called on the from room
    public void EndRoomTransition()
    {
        virtualCamera.gameObject.SetActive(false);
    }

    //called by the door to initiate the room transfer
    public void ConfigureRoomTransition(Door door)
    {
        RoomConnection roomConnection = GetRoomConnectionFromDoor(door);
        if (roomConnection.otherRoom == null) return;

        Door otherDoor = GetOtherRoomDoor(roomConnection.otherRoom);
        if (otherDoor == null) return;

        RoomTransitionData data = new RoomTransitionData(this, roomConnection.otherRoom, door, otherDoor);

        GameController.instance.TransitionToRoom(data);
    }

    RoomConnection GetRoomConnectionFromDoor(Door door)
    {
        for (int i = 0; i < connections.Length; i++)
        {
            if (connections[i].thisRoomDoor == door)
            {
                return connections[i];
            }
        }

        return null;
    }

    Door GetOtherRoomDoor(Room other)
    {
        for(int i = 0; i < other.connections.Length; i++)
        {
            if (other.connections[i].otherRoom == this)
            {
                return other.connections[i].thisRoomDoor;
            }
        }

        return null;
    }
}

//room structs
[System.Serializable]
public class RoomPrefabConnection
{
    public int otherRoomPrefabID;
    public Door thisRoomDoor;
}

public class RoomConnection
{
    public Room otherRoom;
    public Door thisRoomDoor;
}

public struct RoomTransitionData
{
    public Room fromRoom, toRoom;
    public Door fromDoor, toDoor;

    public RoomTransitionData(Room fromRoom, Room toRoom, Door fromDoor, Door toDoor)
    {
        this.fromRoom = fromRoom;
        this.toRoom = toRoom;
        this.fromDoor = fromDoor;
        this.toDoor = toDoor;
    }
}


