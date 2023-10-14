using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct UserInput
{
    public Vector2 movement;
    public ButtonState pause;
    public ButtonState inventory;
    public ButtonState mainAction;
    public ButtonState secondaryAction;
    public ButtonState transform;
}

public enum ButtonState
{
    Off,
    Down,
    Held,
    Release
}