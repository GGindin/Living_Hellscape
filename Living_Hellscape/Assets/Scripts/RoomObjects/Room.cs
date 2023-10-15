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
    CinemachineVirtualCamera virtualCamera;

    [SerializeField]
    RoomPrefabConnection[] prefabConnections;

    [SerializeField]
    ObjectPlacement<PlayerController> playerSpawnPlacement;

    [SerializeField]
    ObjectPlacement<EnemyController>[] enemyPlacements;

    EnemyController[] roomEnemies;

    RoomConnection[] connections;

    public ObjectPlacement<PlayerController> PlayerSpawnPlacement => playerSpawnPlacement;

    public int ID => id;

    private void Awake()
    {
        connections = new RoomConnection[prefabConnections.Length];
        roomEnemies = new EnemyController[enemyPlacements.Length];
    }

    private void Update()
    {
        if(RoomController.Instance.ActiveRoom == this)
        {
            EnemyUpdate();
        }
    }

    private void FixedUpdate()
    {
        if (RoomController.Instance.ActiveRoom == this)
        {
            EnemyFixedUpdate();
        }      
    }

    //called when instantiated / loaded
    public void OnLoadRoom()
    {
        //here we will put stuff like reading and storing save data so that when we enter it, it is the same as when we left it
        //currently not setup yet

        RoomController.Instance.AddRoom(this);
    }

    //called when player starts transitioning into room
    public void OnStartEnterRoom()
    {
        //these methods in the future will read the save data that was loaded before to setup room properly
        //currently just resets to the starting state
        SetupVirtualCamera();
        LoadEnemies();
        LoadAdjacentRooms();       
    }

    //called when actually finished entering room
    public void OnEnterRoom()
    {
        RoomController.Instance.SetActiveRoom(this);
    }

    //called when player finished transition from room
    //called after the OnEnterRoom
    public void OnLeaveRoom()
    {
        TurnOffVirtualCamera();
        RemoveUnNeededRooms();
    }

    //called when room is destroyed
    public void OnUnloadRoom()
    {
        //here we will write save data / reset the room for the next time we load the room
        RoomController.Instance.RemoveRoom(this);
    }

    public void SetupVirtualCamera()
    {
        virtualCamera.Follow = GameController.Instance.PlayerController.transform;
        virtualCamera.gameObject.SetActive(true);
    }

    public void TurnOffVirtualCamera()
    {
        virtualCamera.Follow = null;
        virtualCamera.gameObject.SetActive(false);
    }

    public void StopRoomTransitions()
    {
        for(int i = 0; i < connections.Length; i++)
        {
            var door = connections[i].thisRoomDoor;
            door.SetTriggerToCollider();
        }
    }

    public void AllowRoomTransitions()
    {
        for (int i = 0; i < connections.Length; i++)
        {
            var door = connections[i].thisRoomDoor;
            door.SetTriggerToTrigger();
        }
    }

    //called by the door to initiate the room transfer
    public void ConfigureRoomTransition(Door door)
    {
        RoomConnection roomConnection = GetRoomConnectionFromDoor(door);
        if (roomConnection.otherRoom == null) return;

        Door otherDoor = GetOtherRoomDoor(roomConnection.otherRoom);
        if (otherDoor == null) return;

        RoomTransitionData data = new RoomTransitionData(this, roomConnection.otherRoom, door, otherDoor);

        GameController.Instance.TransitionToRoom(data);
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
            if (other.connections[i] == null) continue;

            if (other.connections[i].otherRoom == this)
            {
                return other.connections[i].thisRoomDoor;
            }
        }

        return null;
    }

    void LoadAdjacentRooms()
    {
        //this loads adjacent rooms so they are ready when the player leaves a room
        for (int i = 0; i < prefabConnections.Length; i++)
        {
            //if already has this setup from a previous load skip
            if (connections[i] != null && connections[i].otherRoom != null) continue;

            //otherwise setup the other room and connections to this room
            Room room = RoomController.Instance.LoadRoomByIndex(prefabConnections[i].otherRoomPrefabID);
            connections[i] = new RoomConnection()
            {
                otherRoom = room,
                thisRoomDoor = prefabConnections[i].thisRoomDoor
            };
            room.SetupRoomConnectionsToRoom(this);
            room.OnLoadRoom();
        }
    }

    //this sets up connections to the active room, so that when you try to leave the connections are set up
    void SetupRoomConnectionsToRoom(Room other)
    {
        for (int i = 0; i < prefabConnections.Length; i++)
        {
            var prefab = prefabConnections[i];
            if (prefab.otherRoomPrefabID == other.id)
            {
                connections[i] = new RoomConnection()
                {
                    otherRoom = other,
                    thisRoomDoor = prefab.thisRoomDoor
                };
            }
        }
    }

    void LoadEnemies()
    {
        //loads objects (right now just enemies)
        for (int i = 0; i < enemyPlacements.Length; i++)
        {
            //get the placement for the enemy
            var placement = enemyPlacements[i];

            //Reset position if already loaded
            if (roomEnemies[i] != null)
            {
                roomEnemies[i].transform.position = placement.Position;
                continue;
            }

            //create enemy if doesn't exist
            //in the future we will remember what happened to enemies and objects
            roomEnemies[i] = Instantiate(placement.prefab, placement.Position, Quaternion.identity, transform);
        }
    }

    void RemoveUnNeededRooms()
    {
        var activeRoom = RoomController.Instance.ActiveRoom;

        for (int i = 0; i < connections.Length; i++)
        {
            var connection = connections[i];
            if (connection.otherRoom != activeRoom)
            {
                connection.otherRoom.OnUnloadRoom();
                Destroy(connection.otherRoom.gameObject);
            }
        }
    }

    void EnemyUpdate()
    {
        for (int i = 0; i < roomEnemies.Length; i++)
        {
            var e = roomEnemies[i];
            if (e)
            {
                e.RoomUpdate();
            }
        }
    }

    void EnemyFixedUpdate()
    {
        for(int i = 0; i < roomEnemies.Length; i++)
        {
            var e = roomEnemies[i];
            if (e)
            {
                e.RoomFixedUpdate();
            }           
        }
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


