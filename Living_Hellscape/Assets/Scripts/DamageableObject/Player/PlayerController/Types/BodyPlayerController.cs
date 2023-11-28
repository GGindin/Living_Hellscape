using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPlayerController : PlayerController
{
    public override void ActivateController()
    {
        boxCollider.isTrigger = false;
    }

    public override void DeactivateController()
    {
        boxCollider.isTrigger = true;
    }

    protected override void SetIgnorePhysics()
    {
        Physics2D.IgnoreLayerCollision(21, 10);
        Physics2D.IgnoreLayerCollision(6, 10);
    }

    protected override void UnSetIgnorePhysics()
    {
        Physics2D.IgnoreLayerCollision(21, 10, false);
        Physics2D.IgnoreLayerCollision(6, 10, false);
    }

    override protected void Awake()
    {
        base.Awake();
    }

    override protected void Move(Vector2 movement)
    {
        base.Move(movement);
        var normInput = movement.normalized;

        Vector2 velocity = normInput * playerStats.Speed * Time.fixedDeltaTime;
        if (velocity.sqrMagnitude > 0)
        {
            AudioController.Instance.PlayWalkSound();
        }
        else
        {
            AudioController.Instance.StopWalkSound();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsActive) return;

        //probably need some check to stop the player from leaving a room whie holding something
        var interactableObject = collision.gameObject.GetComponent<InteractableObject>();
        if (interactableObject)
        {
            if(this.interactableObject == null)
            {
                NotificationBoxController.instance.CloseNotificationBox();
                this.interactableObject = interactableObject;
            }

            NotificationBoxController.instance.OpenNotificationBox("Press K to interact");

            if (!PlayerManager.Instance.PlayerHasControl)
            {
                NotificationBoxController.instance.CloseNotificationBox();
                this.interactableObject.Interact();
            }

            return;
        }

        var enemy = collision.gameObject.GetComponent<EnemyController>();
        if (enemy && !enemy.IsTakingDamage && !IsTakingDamage)
        {
            var contacts = collision.contacts;

            Vector2 damageDir = Vector2.zero;

            foreach (var contact in contacts)
            {
                damageDir += contact.normal;
            }

            var damage = enemy.Damage;
            AddStatusEffect(damage, damageDir.normalized);
        }
    }

    private void OnCollisionExit2D()
    {
        NotificationBoxController.instance.CloseNotificationBox();
    }

    private void OnTriggerExit2D()
    {
        NotificationBoxController.instance.CloseNotificationBox();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!IsActive) return;

        var enemy = collision.gameObject.GetComponent<EnemyController>();
        if (enemy && !enemy.IsTakingDamage && !IsTakingDamage)
        {
            var contacts = collision.contacts;

            Vector2 damageDir = Vector2.zero;

            foreach (var contact in contacts)
            {
                damageDir += contact.normal;
            }

            var damage = enemy.Damage;

            AddStatusEffect(damage, damageDir.normalized);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!IsActive) return;

        var doorTrigger = collision.gameObject.GetComponent<DoorTrigger>();
        if (doorTrigger)
        {
            doorTrigger.SignalTrigger();
            return;
        }

        var drop = collision.gameObject.GetComponent<ItemDrop>();
        if (drop)
        {
            drop.Collect();
            return;
        }

        var interactableObject = collision.gameObject.GetComponent<InteractableObject>();
        if (interactableObject)
        {
            if (this.interactableObject == null)
            {
                NotificationBoxController.instance.CloseNotificationBox();
                this.interactableObject = interactableObject;
            }

            NotificationBoxController.instance.OpenNotificationBox("Press K to interact");

            if (!PlayerManager.Instance.PlayerHasControl)
            {
                NotificationBoxController.instance.CloseNotificationBox();
                this.interactableObject.Interact();
            }

            return;
        }

        StatusRouter statusRouter = collision.gameObject.GetComponent<StatusRouter>();
        if (statusRouter)
        {
            var target = statusRouter.Target;
            if (target)
            {
                Vector2 damageDir = (rb.position - collision.ClosestPoint(rb.position)).normalized;
                var status = target.Statuser.GetStatus(this);
                if (status != null)
                {
                    AddStatusEffect(status, damageDir);
                }
            }
        }
    }
}
