using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Equipment : Item
{
    protected Animator animator;

    [SerializeField]
    protected Damage damage;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public abstract override void SetDirection(Vector2 direction);

    public abstract override void TriggerAction();

    public abstract void EndAction();
}
