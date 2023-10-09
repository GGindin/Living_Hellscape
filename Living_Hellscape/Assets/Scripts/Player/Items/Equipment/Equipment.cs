using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Equipment : MonoBehaviour
{
    protected Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public abstract void SetDirection(Vector2 direction);

    public abstract void TriggerAction();

    public abstract void EndAction();
}
