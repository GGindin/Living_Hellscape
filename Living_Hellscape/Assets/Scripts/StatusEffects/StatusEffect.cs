using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class StatusEffect
{
    [SerializeField]
    protected float duration;

    public abstract StatusEffectType EffectType { get; }

    public float Duration => duration;

    public float CurrentDuration { get; set; }

    public void AddDuration(float duration)
    {
        CurrentDuration += duration;
    }

    public void TickDuration(float delta)
    {
        CurrentDuration -= delta;
    }
}

public enum StatusEffectType
{
    Damage,
    Stun,
    Scare
}
