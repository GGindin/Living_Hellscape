using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputController
{
    //buttons are WASD control for movement
    //ESCAPE for pause menu
    //Plus button for inventory
    //left tab map, if we have it
    //J for main attack
    //k for secondary action
    //space for body swap

    static int lastUpdate = -1;
    static UserInput userInput = new UserInput();

    static class InputConfig
    {
        public static KeyCode Pause = KeyCode.Escape;
        public static KeyCode Inventory = KeyCode.Equals;
        public static KeyCode MainAction = KeyCode.J;
        public static KeyCode secondAction = KeyCode.K;
        public static KeyCode transform = KeyCode.Space;
        public static string Horizontal = "Horizontal";
        public static string Vertical = "Vertical";
    }

    public static UserInput GetUserInput()
    {
        if(lastUpdate == Time.frameCount)
        {
            return userInput;
        }

        GetButtons();
        return userInput;
    }

    static void GetButtons()
    {
        userInput = new UserInput();
        lastUpdate = Time.frameCount;

        userInput.movement.x = GetAxis(InputConfig.Horizontal);
        userInput.movement.y = GetAxis(InputConfig.Vertical);

        userInput.mainAction = GetButton(InputConfig.MainAction);
        userInput.secondaryAction = GetButton(InputConfig.secondAction);

        userInput.transform = GetButton(InputConfig.transform);

        userInput.pause = GetButton(InputConfig.Pause);
        userInput.inventory = GetButton(InputConfig.Inventory);
    }

    static float GetAxis(string axis)
    {
        return Input.GetAxisRaw(axis);
    }

    static ButtonState GetButton(KeyCode keyCode)
    {
        if(Input.GetKeyDown(keyCode))
        {
            return ButtonState.Down;
        }
        else if(Input.GetKey(keyCode))
        {
            return ButtonState.Held;
        }
        else if(Input.GetKeyUp(keyCode))
        {
            return ButtonState.Release;
        }
        else
        {
            return ButtonState.Off;
        }
    }
}
