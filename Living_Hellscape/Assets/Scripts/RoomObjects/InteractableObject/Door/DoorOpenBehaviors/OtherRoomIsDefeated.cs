using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "OtherRoomIsDefeated", menuName = "DoorOpens/Enemies/OtherRoomIsDefeated")]
public class OtherRoomIsDefeated : DoorOpenBehavior
{
    [SerializeField]
    int roomToCheckID;

    public override bool ShouldOpenDoor()
    {
        if (RoomController.Instance.IsRoomLoaded(roomToCheckID))
        {
            var room = RoomController.Instance.GetRoomByID(roomToCheckID);
            if (room != null)
            {
                if (!room.HasActiveEnemies())
                {
                    return true;
                }
            }
        }

        return false;
    }
}
