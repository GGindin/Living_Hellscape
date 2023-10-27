using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DamageableObject : MonoBehaviour
{
    protected Animator animator;

    protected Damage damageFromOther;

    protected int hitID = Animator.StringToHash("hit");

    protected abstract bool CheckHealthForDead();

    protected abstract void ChangeHealth(int delta);

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetupDamage(Damage damage, Vector2 normDirection)
    {
        var newDamage = damage;
        newDamage.SetVectorFromDirection(normDirection);
        TakeDamage(newDamage);
    }

    protected void TakeDamage(Damage damage)
    {
        if (damageFromOther != null && damageFromOther.amount > 0) return;

        damageFromOther = damage;
        ChangeHealth((int)-damageFromOther.amount);
        animator.SetBool(hitID, true);
    }

    protected Vector2 MoveByDamage()
    {
        float t = Mathf.InverseLerp(0, damageFromOther.Duration, damageFromOther.CurrentTime);
        Vector2 offset = Vector2.Lerp(Vector2.zero, damageFromOther.Vector, t) * Time.fixedDeltaTime;
        damageFromOther.CurrentTime -= Time.fixedDeltaTime;

        if (t <= 0)
        {
            damageFromOther = null;
            animator.SetBool(hitID, false);
            if (CheckHealthForDead())
            {
                Destroy(gameObject);
                return Vector2.zero;
            }
        }

        return offset;
    }
}
