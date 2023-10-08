using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPlacement<T>
{
    public GameObject placement;
    public T prefab;

    public Vector2 Position => placement.transform.position;
}
