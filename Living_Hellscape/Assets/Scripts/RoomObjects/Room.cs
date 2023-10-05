using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField]
    RoomConnection[] connections;

    public void RoomTransition(Door door)
    {
        RoomConnection roomConnection = GetRoomConnectionFromDoor(door);
        if (roomConnection.connectedRoom == null) return;

        Door otherDoor = GetOtherRoomDoor(roomConnection.connectedRoom);
        if (otherDoor == null) return;

        GameController.instance.TransitionToRoom(this, roomConnection.connectedRoom, otherDoor);
    }

    RoomConnection GetRoomConnectionFromDoor(Door door)
    {
        for (int i = 0; i < connections.Length; i++)
        {
            if (connections[i].connectedDoor == door)
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
            if (other.connections[i].connectedRoom == this)
            {
                return other.connections[i].connectedDoor;
            }
        }

        return null;
    }
}

[System.Serializable]
public class RoomConnection
{
    public Room connectedRoom;
    public Door connectedDoor;
}


