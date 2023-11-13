using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostWindNPC : NPC
{
    //text 0: is kill the enemies
    //text 1: is give ghost wind
    //text 2: thanks
    [SerializeField]
    Item ghostWindPrefab;

    public override void CheckForTextUpdate()
    {
        if (RoomController.Instance.ActiveRoom.HasActiveEnemies())
        {
            currentText = 0;
            return;
        }

        if (!GameStateController.Instance.HasGhostWind)
        {
            currentText = 1;
        }
        else
        {
            currentText = 2;
        }
    }

    public override void OnFinishText()
    {
        if (currentText == 1)
        {
            currentText = 2;

            var item = Instantiate(ghostWindPrefab);

            // If the object has text attached to it, run the attached textbox creator. This will do nothing otherwise.

            TextBoxController.instance.OpenTextBox("You learned " + ghostWindPrefab.Description);

            PlayerManager.Instance.Inventory.StartAddItem(item);
        }
    }
}
