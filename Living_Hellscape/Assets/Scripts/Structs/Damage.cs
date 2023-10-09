using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Damage
{
    public float amount;
    public float magnitude;

    [SerializeField]
    float duration;
    
    public Vector2 Vector { get; set; }
    public float Duration => duration;
    public float CurrentTime { get; set; }

    public Damage(float amount, float magnitude, float duration)
    {
        this.amount = amount;
        this.magnitude = magnitude;
        this.duration = duration;
        CurrentTime = duration;
    }

    public Damage(Damage damage)
    {
        amount = damage.amount;
        magnitude = damage.magnitude;
        duration = damage.duration;
        CurrentTime = damage.duration;
    }

    public void SetVectorFromDirection(Vector2 direction)
    {
        Vector = direction * magnitude;
    }

    public void Log()
    {
        Debug.Log("Amount " + amount + " Mag " + magnitude + " Duration " + duration + " Vector " + Vector + " Current Time " + CurrentTime);
    }
}
