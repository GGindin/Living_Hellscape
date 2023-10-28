using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Droppable Object Value", menuName = "DroppableObjects/Value")]
public class DroppableObjectValue : DroppableObject
{
    [SerializeField]
    protected int value;

    public override bool IsNull => true;

    int Value { get; }
}