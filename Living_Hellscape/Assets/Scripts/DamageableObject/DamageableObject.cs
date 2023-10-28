using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public abstract class DamageableObject : MonoBehaviour
{
    protected Animator animator;

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
                    var stunEffect = (Stun)statusEffect;
                    AddStunEffect(stunEffect);
                    return;
                case StatusEffectType.Scare:
                    var scareEffect = (Scare)statusEffect;
                    AddScareEffect(scareEffect, normDirection);
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
                    activeEffect.AddDuration(statusEffect.Duration);
                    return;
                case StatusEffectType.Scare:
                    activeEffect.AddDuration(statusEffect.Duration);
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
            CheckHitAnim();
            if (CheckHealthForDead())
            {
                Destroy(gameObject);
                return Vector2.zero;
            }
        }

        return offset;
    }

    protected bool IsStunned()
    {
        var stun = GetStatusOfType(StatusEffectType.Stun);
        if(stun != null)
        {
            stun.TickDuration(Time.deltaTime);
            if(stun.CurrentDuration <= 0f)
            {
                RemoveStatusEffect(stun);
            }
            CheckHitAnim();
            return true;
        }

        return false;
    }

    protected bool IsScared()
    {
        var scare = GetStatusOfType(StatusEffectType.Scare);
        if (scare != null)
        {
            scare.TickDuration(Time.deltaTime);
            if (scare.CurrentDuration <= 0f)
            {
                RemoveStatusEffect(scare);
            }
            CheckHitAnim();
            return true;
        }

        return false;
    }

    protected void TakeDamage(Damage damage)
    {
        ChangeHealth((int)-damage.amount);
        animator.SetBool(hitID, true);
    }

    void CheckHitAnim()
    {
        if(statusEffects.Count > 0)
        {
            return;
        }

        animator.SetBool(hitID, false);
    }

    void AddStunEffect(Stun stun)
    {
        statusEffects.Add(stun);
        animator.SetBool(hitID, true);
    }

    void AddScareEffect(Scare scare, Vector2 normDirection)
    {
        statusEffects.Add(scare);
        scare.Vector = normDirection;
        animator.SetBool(hitID, true);
    }

    void AddDamageEffect(Damage damage, Vector2 normDirection)
    {
        damage.SetVectorFromDirection(normDirection);
        statusEffects.Add(damage);
        TakeDamage(damage);
    }

    protected StatusEffect GetStatusOfType(StatusEffectType statusEffectType)
    {
        for(int i = 0; i < statusEffects.Count; i++)
        {
            if (statusEffects[i].EffectType == statusEffectType) return statusEffects[i];
        }

        return null;
    }
}
