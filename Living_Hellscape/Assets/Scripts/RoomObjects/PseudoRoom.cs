using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PseudoRoom : Room
{
    public override void OnStartEnterRoom()
    {
        ///for the pseudo room we just need to setup the camera
        SetupVirtualCamera();
    }
}
