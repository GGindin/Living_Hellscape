using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostFreeZone : MonoBehaviour
{
    const float ZONEHEALTIME = 1.0f;

    float zoneEntertime = -1;

    public float ZoneEnterTime
    {
        get
        {
            return zoneEntertime;
        }
        set
        {
            zoneEntertime = value;
        }
    }

    private void FixedUpdate()
    {
        if(ZoneEnterTime > 0f)
        {
            ProcessTime();
        }
    }

    private void ProcessTime()
    {
        float delta = Time.time - zoneEntertime;

        if(delta >= ZONEHEALTIME)
        {
            zoneEntertime = Time.time;
            PlayerManager.Instance.GhostInstance.PlayerStats.ChangeCurrentHealth(1);
        }
    }
}
