using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Key : Consumable
{
    [System.Serializable]
    public enum KeyType
    {
        None,
        Small,
        Boss
    }

    public abstract KeyType Type { get; }
}




