using Cinemachine;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour, ISaveableObject
{
    [SerializeField]
    int id;

    [SerializeField]
    string descriptiveName;

    [SerializeField]
    bool defeatAllEnemies;

    [SerializeField]
    bool dontUpdateLastRoomOnEnter;

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

    [SerializeField]
    Tilemap interiorTileMap;

    [SerializeField]
    Tilemap exteriorTileMap;

    HoldableObject[] roomHoldables;

    InteractableObject[] roomInteractables;

    EnemyController[] roomEnemies;

    RoomConnection[] connections;

    bool hasOpenedAllDoors = false;

    public ObjectPlacement<PlayerController> PlayerSpawnPlacement => playerSpawnPlacement;

    public int ID => id;

    public bool DefeateAllEnemies => defeatAllEnemies;

    public Transform DynamicObjectsHolder => dynamicObjectsHolder;

    public bool DontUpdateLastRoomOnEnter => dontUpdateLastRoomOnEnter;

    private void Awake()
    {
        connections = new RoomConnection[prefabConnections.Length];
        roomEnemies = new EnemyController[enemyPlacements.Length];
        roomHoldables = new HoldableObject[holdableObjectPlacements.Length];
        roomInteractables = new InteractableObject[interactableObjectPlacements.Length];
    }

    protected void Update()
    {
        if (GameController.Instance.StopUpdates) return;
        if (RoomController.Instance.ActiveRoom == this)
        {
            if (PlayerManager.Instance.PlayerHasControl || PlayerManager.Instance.StillUpdateRooms)
            {
                EnemyUpdate();
            }

            if (defeatAllEnemies && !HasActiveEnemies() && !hasOpenedAllDoors)
            {
                OpenAllDoors();
            }
        }
    }

    private void FixedUpdate()
    {
        if (GameController.Instance.StopUpdates) return;
        if (RoomController.Instance.ActiveRoom == this)
        {
            if (PlayerManager.Instance.PlayerHasControl || PlayerManager.Instance.StillUpdateRooms)
            {
                EnemyFixedUpdate();
            }

        }      
    }

    public void SaveRoomData()
    {
        GameStorageController.Instance.SavePerm(this);
        GameStorageController.Instance.SaveTemp(this);
    }

    public void ReLoadTempData()
    {
        for(int i = 0; i < roomEnemies.Length; i++)
        {
            if (roomEnemies[i] != null && !(roomEnemies[i] is BossEnemy)) Destroy(roomEnemies[i].gameObject);
        }
        for (int i = 0; i < roomHoldables.Length; i++)
        {
            if (roomHoldables[i] != null) Destroy(roomHoldables[i].gameObject);
        }
        for(int i = 0; i < dynamicObjectsHolder.childCount; i++)
        {
            Destroy(DynamicObjectsHolder.GetChild(i).gameObject);
        }
        GameStorageController.Instance.LoadTemp(this);
    }

    //called when instantiated / loaded
    public virtual void OnLoadRoom()
    {
        //here we will put stuff like reading and storing save data so that when we enter it, it is the same as when we left it
        RoomController.Instance.AddRoom(this);
        GameStorageController.Instance.LoadPerm(this);
        GameStorageController.Instance.LoadTemp(this);
        //gameObject.SetActive(false);
    }

    //called when player starts transitioning into room
    public virtual void OnStartEnterRoom()
    {
        gameObject.SetActive(true);
        //This just sets up the stuff in the active room
        SetupVirtualCamera();
        ResetEnemyPos();
        SetupGhostInteractables();
        LoadAdjacentRooms();
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
        RoomController.Instance.RemoveUnNeededRooms();
        //RemoveUnNeededRooms();
        gameObject.SetActive(false);
    }

    //called when room is destroyed
    public virtual void OnUnloadRoom()
    {
        //here we will write save data / reset the room for the next time we load the room
        RoomController.Instance.RemoveRoom(this);
        SaveRoomData();
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

    public void SetColorOnGhostObjects(Color color)
    {
        for(int i = 0; i < roomInteractables.Length; i++)
        {
            var inter = roomInteractables[i];
            if(inter && inter.GhostObject && inter.SpriteRenderer)
            {
                inter.SpriteRenderer.color = color;
            }
        }
    }

    private void SetupGhostInteractables()
    {
        if(PlayerManager.Instance.Active == PlayerManager.Instance.BodyInstance)
        {
            SetColorOnGhostObjects(new Color(1, 1, 1, 0));
        }
        else
        {
            SetColorOnGhostObjects(Color.white);
        }
    }

    /*
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
    */

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
                    RoomTransitionData roomTransitionData = new RoomTransitionData();
                    roomTransitionData.toDoor = connections[i].thisRoomDoor;
                    roomTransitionData.toRoom = this;
                    roomTransitionData.fromDoor = pseudo.PseudoDoor;
                    roomTransitionData.fromRoom = pseudo;
                    GameController.Instance.PlayerController.SetPosition(pseudo.transform.position);
                    GameController.Instance.TransitionToRoom(roomTransitionData);
                    return;
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

    Door GetOtherRoomDoor(RoomConnection roomConnection)
    {
        var otherRoom = roomConnection.otherRoom;

        for(int i = 0; i < otherRoom.connections.Length; i++)
        {
            if (otherRoom.connections[i] == null) continue;

            if (otherRoom.connections[i].otherRoom == this)
            {
                for(int j = 0; j < connections.Length; j++)
                {
                    if (connections[j].thisRoomDoor.InRoomID == otherRoom.connections[i].thisRoomDoor.InRoomID && 
                        roomConnection.thisRoomDoor.InRoomID == otherRoom.connections[i].thisRoomDoor.InRoomID)
                    {
                        return otherRoom.connections[i].thisRoomDoor;
                    }
                }             
            }
        }

        return null;
    }

    void LoadAdjacentRooms()
    {
        //go through connections and set up with adjacent rooms
        for (int i = 0; i < connections.Length; i++)
        {
            //if already has this setup from a previous load skip
            if (connections[i].otherRoom != null) continue;

            //declare room var
            Room room = null;

            //if pseudo room
            if(prefabConnections[i].pseudoRoomPrefab != null && prefabConnections[i].pseudoRoomPrefab is PseudoRoom)
            {
                //we can just use the prefab because it is going to be placed under the room in the heirarchy
                room = prefabConnections[i].pseudoRoomPrefab;
            }
            else
            {
                //check to see if the room is already loaded
                room = RoomController.Instance.GetRoomByID(prefabConnections[i].otherRoomID);

                //if we do not have the room get the room
                if (!room)
                {
                    room = RoomController.Instance.LoadRoomByIndex(prefabConnections[i].otherRoomID);
                }
            }


            //then add room to connection
            connections[i].otherRoom = room;
            room.SetupRoomConnectionsToRoom(this, connections[i].thisRoomDoor);

            if (!RoomController.Instance.IsRoomLoaded(room.id))
            {
                room.OnLoadRoom();
                room.OffsetRoom(this, connections[i].thisRoomDoor);
                room.gameObject.SetActive(false);
            }          
        }
    }

    //this sets up connections to the active room, so that when you try to leave the connections are set up
    protected void SetupRoomConnectionsToRoom(Room otherRoom, Door otherDoor)
    {
        for (int i = 0; i < prefabConnections.Length; i++)
        {
            var prefab = prefabConnections[i];
            if (prefab.otherRoomID == otherRoom.id && otherDoor.InRoomID == prefab.thisRoomDoor.InRoomID)
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
            //Vector2 interRoomDistance = otherDoor.DoorDirection.DirectionToVector2() * RoomController.INTER_ROOM_DISTANCE +
            //otherDoor.DoorDirection.DirectionToVector2() * 2.0f * 2.0f; //if the door is in the ext tile map then it is only * 2f
            var interRoomDistance = otherDoor.GetDistanceToRoomEdge(otherRoom.interiorTileMap, otherRoom.exteriorTileMap) +
                thisConnection.thisRoomDoor.GetDistanceToRoomEdge(interiorTileMap, exteriorTileMap);
            Vector2 thisDoorDist = transform.position - thisConnection.thisRoomDoor.TargetPos;

            Vector2 totalOffset = otherDoorDist + (otherDoor.DoorDirection.DirectionToVector2() * interRoomDistance) + thisDoorDist;
            transform.position = otherRoom.transform.position + new Vector3(totalOffset.x, totalOffset.y);
        }
    }

    void LoadEnemiesFromDefault()
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

    void LoadHoldableObjectsFromDefault()
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

    private void LoadInteractableObjectsFromDefault()
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

    public bool IsConnectedToActiveRoom()
    {
        var activeRoom = RoomController.Instance.ActiveRoom;

        if (activeRoom == this) return true;

        for (int i = 0; i < prefabConnections.Length; i++)
        {
            var connection = prefabConnections[i];
            if (connection.pseudoRoomPrefab) continue;
            if (connection.otherRoomID == activeRoom.ID)
            {
                return true;
            }
        }

        return false;
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

    private void ResetEnemyPos()
    {
        for (int i = 0; i < roomEnemies.Length; i++)
        {
            var e = roomEnemies[i];
            if (e)
            {
                e.transform.position = enemyPlacements[i].Position;
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
            //this shouldn't ever be the case
            //no need to write a null value because it should always be there
            if (connections[i] == null) continue;

            var door = connections[i].thisRoomDoor;
            door.SavePerm(writer);
        }
        for (int i = 0; i < roomInteractables.Length; i++)
        {
            //this shouldn't ever be the case
            //no need to write a null value because it should always be there
            if (roomInteractables[i] == null) continue;

            var interactable = roomInteractables[i];
            interactable.SavePerm(writer);
        }
        //for now we do not save any enemies perm data because we do not have bosses
        for (int i = 0; i < enemyPlacements.Length; i++)
        {           
            var prefab = enemyPlacements[i].prefab;
            if(prefab is BossEnemy)
            {
                if (roomEnemies[i] == null)
                {
                    //if it is null and a boss that means it is a defeated boss and needs to write a -1 to show that
                    //it is dead
                    writer.WriteInt(-1);
                }
                else
                {
                    prefab.SavePerm(writer);
                }
            }
            
        }
    }

    //these loops nned to be based on reads from data
    public void LoadPerm(GameDataReader reader)
    {
        //load doors and connections
        //the other room will not be available yet just leave null
        for (int i = 0; i < connections.Length; i++)
        {
            //if the door is not set up yet assign it and create the connection
            if (connections[i] == null)
            {
                connections[i] = new RoomConnection();
                connections[i].thisRoomDoor = prefabConnections[i].thisRoomDoor;
            }

            //then load any data on that door
            if (reader != null)
            {
                connections[i].thisRoomDoor.LoadPerm(reader);
            }
        }

        //load and setup interactable obejcts
        for (int i = 0; i < roomInteractables.Length; i++)
        {
            //if doesn't exist instantiate and set position
            if (roomInteractables[i] == null)
            {
                var placement = interactableObjectPlacements[i];
                roomInteractables[i] = Instantiate(placement.prefab, placement.Position, Quaternion.identity, transform);
            }

            //then load any data on that door
            if (reader != null)
            {
                roomInteractables[i].LoadPerm(reader);
            }
        }

        //load and setup any perm state enemies, that is bosses
        for (int i = 0; i < enemyPlacements.Length; i++)
        {
            
             //currently no bosses so does not happen
            if (enemyPlacements[i].prefab is BossEnemy)
            {
                if(reader == null)
                {
                    //if no reader its not dead, instantiate
                    var placement = enemyPlacements[i];
                    roomEnemies[i] = Instantiate(placement.prefab, placement.Position, Quaternion.identity, transform);
                }
                else
                {
                    var value = reader.ReadInt();
                    if (value >= 0)
                    {
                        //if not dead instantiate
                        var placement = enemyPlacements[i];
                        roomEnemies[i] = Instantiate(placement.prefab, placement.Position, Quaternion.identity, transform);

                        roomEnemies[i].LoadPerm(reader);
                    }
                }

            }

            
        }
    }

    public void SaveTemp(GameDataWriter writer)
    {
        for (int i = 0; i < roomEnemies.Length; i++)
        {
            // the bosses do not have temp data so they can just be ignored
            if (enemyPlacements[i].prefab is BossEnemy)
            {
                continue;
            }
            
            if (roomEnemies[i] == null)
            {
                //in future need to not write value for boss enemies
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

    public void LoadTemp(GameDataReader reader)
    {
        //load and setup any temp state enemies, that is not bosses
        for (int i = 0; i < enemyPlacements.Length; i++)
        {
            // the bosses do not have temp data so they can just be ignored
            if (enemyPlacements[i].prefab is BossEnemy)
            {
                continue;
            }
            
            //if doesn't exist instantiate and set position

            if(reader == null)
            {
                //instantiate and load data
                var placement = enemyPlacements[i];
                roomEnemies[i] = Instantiate(placement.prefab, placement.Position, Quaternion.identity, transform);
            }
            else
            {
                var value = reader.ReadInt();
                if (value >= 0)
                {
                    //instantiate and load data
                    var placement = enemyPlacements[i];
                    roomEnemies[i] = Instantiate(placement.prefab, placement.Position, Quaternion.identity, transform);

                    //then load any data
                    roomEnemies[i].LoadTemp(reader);
                }
            }
        }

        for (int i = 0; i < holdableObjectPlacements.Length; i++)
        {
            if (reader == null)
            {
                //instantiate and load data
                var placement = holdableObjectPlacements[i];
                roomHoldables[i] = Instantiate(placement.prefab, placement.Position, Quaternion.identity, transform);
            }
            else
            {
                var value = reader.ReadInt();
                if (value >= 0)
                {
                    //instantiate and load data
                    var placement = holdableObjectPlacements[i];
                    roomHoldables[i] = Instantiate(placement.prefab, placement.Position, Quaternion.identity, transform);

                    //then load any data
                    roomHoldables[i].LoadTemp(reader);
                }
            }
        }
    }
}

//room structs
[System.Serializable]
public class RoomPrefabConnection
{
    public int otherRoomID;
    public Room pseudoRoomPrefab;
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


