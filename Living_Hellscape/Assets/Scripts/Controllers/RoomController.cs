using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public static RoomController Instance { get; private set; }

    [SerializeField]
    int currentRoomOverrideIndex = -1;

    //[SerializeField]
    //Room[] roomPrefabs;

    Room activeRoom;  

    HashSet<Room> spawnedRooms = new HashSet<Room>();

    public Room ActiveRoom => activeRoom;

    private void Awake()
    {
        Instance = this;       
    }

    public void Initialize()
    {
        LoadDefaultRoom();
    }

    public void SetActiveRoom(Room room)
    {
        activeRoom = room;

        if(room != null && !room.DontUpdateLastRoomOnEnter)
        {
            currentRoomOverrideIndex = activeRoom.ID;
            GameStateController.Instance.CurrentRoomIndex = activeRoom.ID;
        }
    }

    public void RemoveUnNeededRooms()
    {
        List<Room> removeList = new List<Room>();

        foreach(Room room in spawnedRooms)
        {
            if (!room.IsConnectedToActiveRoom())
            {
                if (!removeList.Contains(room))
                {
                    removeList.Add(room);
                }               
            }
        }

        for(int i = 0; i < removeList.Count; i++)
        {
            var room = removeList[i];
            room.OnUnloadRoom();
            Destroy(room.gameObject);
        }
    }

    public Room LoadRoomByIndex(int index)
    {
        return InstantiateRoomFromID(index);

        /*
        if (index < 0 || index >= roomPrefabs.Length)
        {
            return null;
        }
        else
        {
            return Instantiate(roomPrefabs[index]);
        }
        */
    }

    private void LoadDefaultRoom()
    {
        var indexToLoad = currentRoomOverrideIndex;
        if(indexToLoad < 0)
        {
            indexToLoad = GameStateController.Instance.CurrentRoomIndex;
        }

        activeRoom = InstantiateRoomFromID(indexToLoad);
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
            GameStorageController.Instance.SavePerm(sr);
            GameStorageController.Instance.SaveTemp(sr);
            Destroy(sr.gameObject);
        }
        spawnedRooms.Clear();

        //then load in the new data
        int roomID = floorTransitionData.connectedRoomPrefab.ID;
        
        Room room = InstantiateRoomFromID(roomID);
        //Room room = Instantiate(roomPrefabs[roomID]);
        room.OnLoadRoom();
        SetActiveRoom(room);
        room.OnStartEnterRoom();
        room.ConfigureFromPseudoRoomTransitionByConnectionID(floorTransitionData.pseudoRoomConnectionID);
    }

    public void StartFadeIn()
    {
        StartCoroutine(ProcessFadeIn());
    }

    public void FadeInImmediate()
    {
        foreach (Room r in spawnedRooms)
        {
            r.SetColorOnGhostObjects(Color.white);
        }
    }

    public void FadeOutImmediate()
    {
        foreach (Room r in spawnedRooms)
        {
            r.SetColorOnGhostObjects(new Color(1, 1, 1, 0));
        }
    }

    IEnumerator ProcessFadeIn()
    {
        float alpha = 0f;
        Color color = Color.white;
        color.a = alpha;

        float duration = GhostWorldFilterController.Instance.TransitionLength;
        float current = 0;

        while(current < duration)
        {
            current += Time.deltaTime;
            float t = Mathf.InverseLerp(0f, duration, current);
            alpha = Mathf.Lerp(0.0f, 1.0f, t);
            color.a = alpha;

            foreach(Room r in spawnedRooms)
            {
                r.SetColorOnGhostObjects(color);
            }

            yield return null;
        }

        foreach (Room r in spawnedRooms)
        {
            r.SetColorOnGhostObjects(Color.white);
        }
    }

    public void StartFadeOut()
    {
        StartCoroutine(ProcessFadeOut());
    }

    IEnumerator ProcessFadeOut()
    {
        float alpha = 1f;
        Color color = Color.white;

        float duration = GhostWorldFilterController.Instance.TransitionLength;
        float current = 0;

        while (current < duration)
        {
            current += Time.deltaTime;
            float t = Mathf.InverseLerp(0f, duration, current);
            alpha = Mathf.Lerp(1.0f, 0.0f, t);
            color.a = alpha;

            foreach (Room r in spawnedRooms)
            {
                r.SetColorOnGhostObjects(color);
            }

            yield return null;
        }

        color.a = 0f;

        foreach (Room r in spawnedRooms)
        {
            r.SetColorOnGhostObjects(color);
        }
    }

    public void SaveRoomData()
    {
        foreach(Room room in spawnedRooms)
        {
            room.SaveRoomData();
        }
    }

    Room InstantiateRoomFromID(int id)
    {
        var roomPrefab = Resources.Load<Room>("Rooms/room" + id);

        return Instantiate(roomPrefab);
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
