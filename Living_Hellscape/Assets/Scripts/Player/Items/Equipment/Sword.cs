using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Equipment
{
    [SerializeField]
    Damage damage;

    public Damage Damage => new Damage(damage);


    int xDirID = Animator.StringToHash("xDir");
    int yDirID = Animator.StringToHash("yDir");
    int strikeID = Animator.StringToHash("strike");

    bool isActing = false;

    public override void SetDirection(Vector2 direction)
    {
        if (!isActing)
        {
            animator.SetFloat(xDirID, direction.x);
            animator.SetFloat(yDirID, direction.y);
        }
    }

    public override void TriggerAction()
    {
        isActing = true;
        animator.SetTrigger(strikeID);
    }

    public override void EndAction()
    {
        isActing = false;
    }
}
