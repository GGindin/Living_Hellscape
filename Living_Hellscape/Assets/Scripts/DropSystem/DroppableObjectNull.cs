using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Droppable Object Null", menuName = "DroppableObjects/Null")]
public class DroppableObjectNull : DroppableObject
{
    public override bool IsNull => true;
}