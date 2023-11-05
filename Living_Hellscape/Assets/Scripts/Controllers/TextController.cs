using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextController : MonoBehaviour
{
    public static TextController Instance { get; private set; }

    public bool Active => enabled;


    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Active)
        {
            UserInput userInput = InputController.GetUserInput();
            if (userInput.mainAction == ButtonState.Down)
            {
                enabled = false;
                GameController.Instance.SetStopUpdates(false);
            }
        }
    }

    public void SetText(string text)
    {
        Debug.Log(text);
        enabled = true;
        GameController.Instance.SetStopUpdates(true);
    }
}
