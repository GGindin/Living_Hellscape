using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingshotNPC : NPC
{
    //text 0: give sling shot
    //text 1: say thanks
    [SerializeField]
    Item slingShotPrefab;

    public override void CheckForTextUpdate()
    {

        if (!GameStateController.Instance.HasSlingShot)
        {
            currentText = 0;
        }
        else
        {
            currentText = 1;
        }
    }

    public override void OnFinishText()
    {
        if (!GameStateController.Instance.HasSlingShot)
        {
            var item = Instantiate(slingShotPrefab);

            // If the object has text attached to it, run the attached textbox creator. This will do nothing otherwise.

            TextBoxController.instance.OpenTextBox("You got a " + slingShotPrefab.Description);

            PlayerManager.Instance.Inventory.StartAddItem(item);
        }
    }
}
