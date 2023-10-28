using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stun : StatusEffect
{
    public override StatusEffectType EffectType => StatusEffectType.Stun;

    public Stun(Stun stun)
    {
        duration = stun.duration;
        CurrentDuration = stun.duration;
    }
}
