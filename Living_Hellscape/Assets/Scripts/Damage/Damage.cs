using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Damage : StatusEffect
{
    public float amount;
    public float magnitude;
    
    public Vector2 Vector { get; set; }

    public Damage(float amount, float magnitude, float duration)
    {
        this.amount = amount;
        this.magnitude = magnitude;
        this.duration = duration;
        CurrentDuration = duration;
    }

    public Damage(Damage damage)
    {
        amount = damage.amount;
        magnitude = damage.magnitude;
        duration = damage.duration;
        CurrentDuration = damage.duration;
    }

    public void SetVectorFromDirection(Vector2 direction)
    {
        Vector = direction * magnitude;
    }
}
