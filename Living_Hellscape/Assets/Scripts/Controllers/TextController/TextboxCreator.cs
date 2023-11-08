using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextboxCreator : MonoBehaviour
{
    public string text;

    public void StartTextEvent()
    {
        TextBoxController.instance.OpenTextBox(text);
    }

    void Update()
    {
        if (TextBoxController.instance.isFinished == true) // Leaving this here because it could potentially be useful to close the text box from outside the object
        {
            Debug.Log("Closed textbox");
            TextBoxController.instance.CloseTextBox(); 
        }
    }
}
