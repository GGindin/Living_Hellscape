using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostWind : GhostEquipment, IStatuser
{
    [SerializeField]
    LayerMask EnemyBodyLayer;

    [SerializeField]
    LayerMask EnemyGhostLayer;

    [SerializeField]
    Scare scare;

    [SerializeField]
    Stun stun;

    int xDirID = Animator.StringToHash("xDir");
    int yDirID = Animator.StringToHash("yDir");
    int strikeID = Animator.StringToHash("strike");

    bool isActing = false;

    public override void EndAction()
    {
        isActing = false;
    }

    public StatusEffect GetStatus(DamageableObject recievingObject)
    {
        if((EnemyBodyLayer & 1 << recievingObject.gameObject.layer) != 0)
        {
            return new Stun(stun);
        }
        else if((EnemyGhostLayer & 1 << recievingObject.gameObject.layer) != 0)
        {
            return new Scare(scare);
        }

        return null;
    }

    public override void SetDirection(Vector2 direction)
    {
        if (isActing) return;

        animator.SetFloat(xDirID, direction.x);
        animator.SetFloat(yDirID, direction.y);
    }

    public override void TriggerAction()
    {
        isActing = true;
        animator.SetTrigger(strikeID);
    }


}
