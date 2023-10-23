using Cinemachine;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Room : MonoBehaviour, ISaveableObject
{
    [SerializeField]
    int id;

    [SerializeField]
    bool defeatAllEnemies;

    [SerializeField]
    CinemachineVirtualCamera virtualCamera;

    [SerializeField]
    RoomPrefabConnection[] prefabConnections;

    [SerializeField]
    ObjectPlacement<PlayerController> playerSpawnPlacement;

    [SerializeField]
    ObjectPlacement<EnemyController>[] enemyPlacements;

    [SerializeField]
    ObjectPlacement<HoldableObject>[] holdableObjectPlacements;

    [SerializeField]
    ObjectPlacement<InteractableObject>[] interactableObjectPlacements;

    [SerializeField]
    Transform dynamicObjectsHolder;

    HoldableObject[] roomHoldables;

    InteractableObject[] roomInteractables;

    EnemyController[] roomEnemies;

    RoomConnection[] connections;

    bool hasOpenedAllDoors = false;

    public ObjectPlacement<PlayerController> PlayerSpawnPlacement => playerSpawnPlacement;

    public int ID => id;

    public bool DefeateAllEnemies => defeatAllEnemies;

    public Transform DynamicObjectsHolder => dynamicObjectsHolder;

    private void Awake()
    {
        connections = new RoomConnection[prefabConnections.Length];
        roomEnemies = new EnemyController[enemyPlacements.Length];
        roomHoldables = new HoldableObject[holdableObjectPlacements.Length];
        roomInteractables = new InteractableObject[interactableObjectPlacements.Length];
    }

    protected void Update()
    {
        if(RoomController.Instance.ActiveRoom == this)
        {
            EnemyUpdate();
            if (defeatAllEnemies && !HasActiveEnemies() && !hasOpenedAllDoors)
            {
                OpenAllDoors();
            }
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
    public virtual void OnLoadRoom()
    {
        //here we will put stuff like reading and storing save data so that when we enter it, it is the same as when we left it
        //currently not setup yet

        RoomController.Instance.AddRoom(this);
    }

    //called when player starts transitioning into room
    public virtual void OnStartEnterRoom()
    {
        //these methods in the future will read the save data that was loaded before to setup room properly
        //currently just resets to the starting state
        //some of these methods need to move to onload
        //because rooms can get loaded without being entered and then get unloaded, which then
        //saves eveything as null becasue none of these load methods ever got called
        //so the load methods should get moved to onload, except for maybe the loadadjrooms method
        //however that causes problems when you leave and go back without unloading
        //probably the better way to handle would be to use the IO at the same times as setting things up so 
        //they are in sync
        //probably do saves in leave and not unload
        SetupVirtualCamera();
        LoadEnemies();
        LoadHoldableObjects();
        LoadInteractableObjects();
        LoadAdjacentRooms();
        GameStorageController.Instance.LoadPerm(this);
        GameStorageController.Instance.LoadTemp(this);
    }



    //called when actually finished entering room
    public virtual void OnEnterRoom()
    {
        RoomController.Instance.SetActiveRoom(this);
    }

    //called when player finished transition from room
    //called after the OnEnterRoom
    public virtual void OnLeaveRoom()
    {
        hasOpenedAllDoors = false;
        CloseAllDoors();
        RemoveDynamicObjects();
        TurnOffVirtualCamera();
        RemoveUnNeededRooms();
    }

    //called when room is destroyed
    public virtual void OnUnloadRoom()
    {
        //here we will write save data / reset the room for the next time we load the room
        RoomController.Instance.RemoveRoom(this);
        GameStorageController.Instance.SavePerm(this);
        GameStorageController.Instance.SaveTemp(this);
    }

    public virtual void SetupVirtualCamera()
    {
        virtualCamera.Follow = GameController.Instance.PlayerController.transform;
        virtualCamera.gameObject.SetActive(true);
    }

    public virtual void TurnOffVirtualCamera()
    {
        virtualCamera.Follow = null;
        virtualCamera.gameObject.SetActive(false);
    }

    public void WarpVirtualCamera(Vector3 delta)
    {
        //virtualCamera.PreviousStateIsValid = false;
        virtualCamera.OnTargetObjectWarped(GameController.Instance.PlayerController.transform, delta);
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

    public bool HasActiveEnemies()
    {
        for(int i = 0; i < roomEnemies.Length; i++)
        {
            if (roomEnemies[i])
            {
                return true;
            }
        }

        return false;
    }

    public void OpenAllDoors()
    {
        for (int i = 0; i < connections.Length; i++)
        {
            var door = connections[i].thisRoomDoor;
            door.OpenDoor();
        }
    }

    public void CloseAllDoors()
    {
        for (int i = 0; i < connections.Length; i++)
        {
            var door = connections[i].thisRoomDoor;
            door.CloseDoor();
        }
    }

    public PseudoRoom FindPseudoRoomByConnectionID(int id)
    {
        for(int i = 0; i < connections.Length; i++)
        {
            var room = connections[i].otherRoom;
            if(room is PseudoRoom)
            {
                var pseudo = (PseudoRoom)room;
                if(id == pseudo.PseudoRoomConnection.pseudoRoomID)
                {
                    return pseudo;
                }
            }
        }

        return null;
    }

    public void ConfigureFromPseudoRoomTransitionByConnectionID(int id)
    {
        for (int i = 0; i < connections.Length; i++)
        {
            var room = connections[i].otherRoom;
            if (room is PseudoRoom)
            {
                var pseudo = (PseudoRoom)room;
                if (id == pseudo.PseudoRoomConnection.pseudoRoomID)
                {
                    //this shouldnt be thisRoomDoor, needs to be the pseudo room door, but thats not how this method works
                    //because this is being called on the actual first room on the next floor
                    RoomTransitionData roomTransitionData = new RoomTransitionData();
                    roomTransitionData.toDoor = connections[i].thisRoomDoor;
                    roomTransitionData.toRoom = this;
                    roomTransitionData.fromDoor = pseudo.PseudoDoor;
                    roomTransitionData.fromRoom = pseudo;
                    GameController.Instance.TransitionToRoom(roomTransitionData);
                }
            }
        }
    }

    //called by the door to initiate the room transfer
    public virtual void ConfigureRoomTransition(Door door)
    {
        RoomConnection roomConnection = GetRoomConnectionFromDoor(door);
        if (roomConnection.otherRoom == null) return;

        Door otherDoor = GetOtherRoomDoor(roomConnection);
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

    Door GetOtherRoomDoor(RoomConnection other)
    {
        for(int i = 0; i < other.otherRoom.connections.Length; i++)
        {
            if (other.otherRoom.connections[i] == null) continue;

            if (other.otherRoom.connections[i].otherRoom == this)
            {
                for(int j = 0; j < connections.Length; j++)
                {
                    if (connections[j].thisRoomDoor.InRoomID == other.otherRoom.connections[i].thisRoomDoor.InRoomID)
                    {
                        return other.otherRoom.connections[i].thisRoomDoor;
                    }
                }             
            }
        }

        return null;
    }

    void LoadAdjacentRooms()
    {
        //this loads adjacent rooms so they are ready when the player leaves a room
        for (int i = 0; i < prefabConnections.Length; i++)
        {
            //if already has this setup from a previous load skip, and the room exists
            if (connections[i] != null && connections[i].otherRoom != null) continue;

            //declare room var
            Room room = null;

            //if pseudo room
            if(prefabConnections[i].otherRoomPrefab is PseudoRoom)
            {
                //we can just use the prefab because it is going to be placed under the room in the heirarchy
                room = prefabConnections[i].otherRoomPrefab;
            }
            else
            {
                //check to see if the room is already loaded
                room = RoomController.Instance.GetRoomByID(prefabConnections[i].otherRoomPrefab.ID);

                //if we do not have the room get the room
                if (!room)
                {
                    room = RoomController.Instance.LoadRoomByIndex(prefabConnections[i].otherRoomPrefab.ID);
                }
            }


            //then set up the connection
            connections[i] = new RoomConnection()
            {
                otherRoom = room,
                thisRoomDoor = prefabConnections[i].thisRoomDoor
            };
            room.SetupRoomConnectionsToRoom(this, connections[i].thisRoomDoor);

            if (!RoomController.Instance.IsRoomLoaded(room.id))
            {
                room.OnLoadRoom();
                room.OffsetRoom(this, connections[i].thisRoomDoor);
            }          
        }
    }

    //this sets up connections to the active room, so that when you try to leave the connections are set up
    protected void SetupRoomConnectionsToRoom(Room otherRoom, Door otherDoor)
    {
        for (int i = 0; i < prefabConnections.Length; i++)
        {
            var prefab = prefabConnections[i];
            if (prefab.otherRoomPrefab.id == otherRoom.id && otherDoor.InRoomID == prefab.thisRoomDoor.InRoomID)
            {
                connections[i] = new RoomConnection()
                {
                    otherRoom = otherRoom,
                    thisRoomDoor = prefab.thisRoomDoor
                };
            }
        }
    }

    void OffsetRoom(Room otherRoom, Door otherDoor)
    {

        if (this is PseudoRoom) return;

        RoomConnection thisConnection = null;

        for(int i = 0; i < connections.Length; i++)
        {
            if (connections[i] == null) continue;

            if (connections[i].otherRoom == otherRoom && connections[i].thisRoomDoor.InRoomID == otherDoor.InRoomID)
            {
                thisConnection = connections[i];
                break;
            }
        }

        if (thisConnection != null)
        {
            Vector2 otherDoorDist = otherDoor.TargetPos - otherRoom.transform.position;
            Vector2 interRoomDistance = otherDoor.DoorDirection.DirectionToVector2() * RoomController.INTER_ROOM_DISTANCE +
                otherDoor.DoorDirection.DirectionToVector2() * 2.0f;
            Vector2 thisDoorDist = transform.position - thisConnection.thisRoomDoor.TargetPos;

            Vector2 totalOffset = otherDoorDist + interRoomDistance + thisDoorDist;
            transform.position = otherRoom.transform.position + new Vector3(totalOffset.x, totalOffset.y);
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

    void LoadHoldableObjects()
    {
        //loads objects (right now just enemies)
        for (int i = 0; i < holdableObjectPlacements.Length; i++)
        {
            //get the placement for the enemy
            var placement = holdableObjectPlacements[i];

            //Reset position if already loaded
            if (roomHoldables[i] != null)
            {
                roomHoldables[i].transform.position = placement.Position;
                continue;
            }

            //create enemy if doesn't exist
            //in the future we will remember what happened to enemies and objects
            roomHoldables[i] = Instantiate(placement.prefab, placement.Position, Quaternion.identity, transform);
        }
    }

    private void LoadInteractableObjects()
    {
        //loads objects (right now just enemies)
        for (int i = 0; i < interactableObjectPlacements.Length; i++)
        {
            //get the placement for the enemy
            var placement = interactableObjectPlacements[i];

            //Reset position if already loaded
            if (roomInteractables[i] != null)
            {
                roomInteractables[i].transform.position = placement.Position;
                continue;
            }

            //create enemy if doesn't exist
            //in the future we will remember what happened to enemies and objects
            roomInteractables[i] = Instantiate(placement.prefab, placement.Position, Quaternion.identity, transform);
        }
    }

    void RemoveDynamicObjects()
    {
        int childCount = dynamicObjectsHolder.childCount;

        for(int i = childCount - 1; i >= 0; i--)
        {
            var child = dynamicObjectsHolder.GetChild(i);
            Destroy(child.gameObject);
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

    public string GetFileName()
    {
        return "room_" + id;
    }

    public void SavePerm(GameDataWriter writer)
    {
        for (int i = 0; i < connections.Length; i++)
        {
            if (connections[i] == null) continue;

            var door = connections[i].thisRoomDoor;
            door.SavePerm(writer);           
        }
        for (int i = 0; i < roomInteractables.Length; i++)
        {
            if (roomInteractables[i] == null) continue;

            var interactable = roomInteractables[i];
            interactable.SavePerm(writer);
        }
    }

    public void SaveTemp(GameDataWriter writer)
    {
        for (int i = 0; i < roomEnemies.Length; i++)
        {           
            if (roomEnemies[i] == null)
            {
                writer.WriteInt(-1);
            }
            else
            {
                roomEnemies[i].SaveTemp(writer);
            }
        }
        for (int i = 0; i < roomHoldables.Length; i++)
        {
            if (roomHoldables[i] == null)
            {
                writer.WriteInt(-1);
            }
            else
            {
                roomHoldables[i].SaveTemp(writer);
            }
        }
    }

    public void LoadPerm(GameDataReader reader)
    {
        for (int i = 0; i < connections.Length; i++)
        {
            if (connections[i] == null) continue;

            var door = connections[i].thisRoomDoor;
            door.LoadPerm(reader);
        }
        for (int i = 0; i < roomInteractables.Length; i++)
        {
            if (roomInteractables[i] == null) continue;

            var interactable = roomInteractables[i];
            interactable.LoadPerm(reader);
        }
    }

    public void LoadTemp(GameDataReader reader)
    {
        for (int i = 0; i < roomEnemies.Length; i++)
        {
            var isNull = reader.ReadInt();
            if (isNull < 0)
            {
                Destroy(roomEnemies[i].gameObject);
                roomEnemies[i] = null;
            }
            else
            {
                roomEnemies[i].LoadTemp(reader);
            }
        }
        for (int i = 0; i < roomHoldables.Length; i++)
        {
            var isNull = reader.ReadInt();
            if (isNull < 0)
            {
                Destroy(roomHoldables[i].gameObject);
                roomHoldables[i] = null;
            }
            else
            {
                roomHoldables[i].LoadTemp(reader);
            }
        }
    }
}

//room structs
[System.Serializable]
public class RoomPrefabConnection
{
    public Room otherRoomPrefab;
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


