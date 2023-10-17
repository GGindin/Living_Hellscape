using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LayerUtil
{
    public static int LayerMaskToLayer(LayerMask layerMask)
    {
        int layer = 0;
        int testValue = 1;

        while(testValue <= layerMask.value)
        {
            if((layerMask.value & testValue) != 0)
            {
                break;
            }

            layer += 1;
            testValue <<= 1;
        }

        return layer;
    }
}
