using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : Consumable
{
    [System.Serializable]
    public enum KeyType
    {
        None,
        Small,
        Boss
    }

    public KeyType keyType;
}




