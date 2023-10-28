using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Scare : StatusEffect
{
    public override StatusEffectType EffectType => StatusEffectType.Scare;

    public Vector2 Vector { get; set; }

    public Scare(Scare scare)
    {
        duration = scare.duration;
        CurrentDuration = scare.duration;
    }
}
