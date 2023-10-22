using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PseudoDoor : Door
{

    protected override void Awake()
    {
        room = GetComponent<PseudoRoom>();
    }

    protected override void SetDoorSprite() { }

    public override void CloseDoor() { }
}
