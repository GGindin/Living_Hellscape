using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DamageableObject : MonoBehaviour
{
    protected Animator animator;

    //protected Damage damageFromOther;

    protected int hitID = Animator.StringToHash("hit");

    protected List<StatusEffect> statusEffects = new List<StatusEffect>();

    protected abstract bool CheckHealthForDead();

    protected abstract void ChangeHealth(int delta);

    public bool IsTakingDamage => GetStatusOfType(StatusEffectType.Damage) != null;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void AddStatusEffect(StatusEffect statusEffect, Vector2 normDirection)
    {
        var activeEffect = GetStatusOfType(statusEffect.EffectType);

        if(activeEffect == null)
        {
            switch(statusEffect.EffectType)
            {
                case StatusEffectType.Damage:
                    var damageEffect = (Damage)statusEffect;
                    AddDamageEffect(damageEffect, normDirection);
                    return;
                case StatusEffectType.Stun:
                    return;
                case StatusEffectType.Scare:
                    return;
            }
        }
        else
        {
            switch (statusEffect.EffectType)
            {
                case StatusEffectType.Damage:
                    return;
                case StatusEffectType.Stun:
                    return;
                case StatusEffectType.Scare:
                    return;
            }
        }
    }

    public void RemoveStatusEffect(StatusEffect statusEffect)
    {
        for(int i = 0; i < statusEffects.Count; i++)
        {
            if (statusEffects[i].EffectType == statusEffect.EffectType)
            {
                statusEffects.RemoveAt(i);
                return;
            }
        }
    }

    protected Vector2 MoveByDamage()
    {
        var damageStatus = GetStatusOfType(StatusEffectType.Damage);

        if (damageStatus == null) return Vector2.zero;

        var damageFromOther = (Damage)damageStatus;

        float t = Mathf.InverseLerp(0, damageFromOther.Duration, damageFromOther.CurrentDuration);
        Vector2 offset = Vector2.Lerp(Vector2.zero, damageFromOther.Vector, t) * Time.fixedDeltaTime;
        damageFromOther.TickDuration(Time.fixedDeltaTime);

        if (t <= 0)
        {
            RemoveStatusEffect(damageFromOther);
            animator.SetBool(hitID, false);
            if (CheckHealthForDead())
            {
                Destroy(gameObject);
                return Vector2.zero;
            }
        }

        return offset;
    }

    protected void TakeDamage(Damage damage)
    {
        ChangeHealth((int)-damage.amount);
        animator.SetBool(hitID, true);
    }

    void AddDamageEffect(Damage damage, Vector2 normDirection)
    {
        damage.SetVectorFromDirection(normDirection);
        statusEffects.Add(damage);
        TakeDamage(damage);
    }

    StatusEffect GetStatusOfType(StatusEffectType statusEffectType)
    {
        for(int i = 0; i < statusEffects.Count; i++)
        {
            if (statusEffects[i].EffectType == statusEffectType) return statusEffects[i];
        }

        return null;
    }
}
