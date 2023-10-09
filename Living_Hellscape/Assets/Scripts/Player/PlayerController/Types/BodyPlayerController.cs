using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPlayerController : PlayerController
{
    public override void ActivateController()
    {
        boxCollider.isTrigger = false;
        isActive = true;
    }

    public override void DeactivateController()
    {
        boxCollider.isTrigger = true;
        isActive = false;
    }

    override protected void Awake()
    {
        base.Awake();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isActive) return;

        var door = collision.gameObject.GetComponent<Door>();
        if (door)
        {
            door.OperateDoor();
            return;
        }

        var enemy = collision.gameObject.GetComponent<EnemyController>();
        if (enemy && damageFromOther == null && !enemy.isTakingDamage)
        {
            var contacts = collision.contacts;

            Vector2 damageDir = Vector2.zero;

            foreach (var contact in contacts)
            {
                damageDir += contact.normal;
            }

            var damage = enemy.Damage;
            damage.SetVectorFromDirection(damageDir.normalized);

            TakeDamage(damage);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!isActive) return;

        var enemy = collision.gameObject.GetComponent<EnemyController>();
        if (enemy && damageFromOther == null && !enemy.isTakingDamage)
        {
            var contacts = collision.contacts;

            Vector2 damageDir = Vector2.zero;

            foreach (var contact in contacts)
            {
                damageDir += contact.normal;
            }

            var damage = enemy.Damage;
            damage.SetVectorFromDirection(damageDir.normalized);

            TakeDamage(damage);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!isActive) return;

        var doorTrigger = collision.gameObject.GetComponent<DoorTrigger>();
        if (doorTrigger)
        {
            doorTrigger.SignalTrigger();
        }
    }
}
