using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostPlayerController : PlayerController
{
    bool hasLeftPlayer = false;

    public override void ActivateController()
    {
        gameObject.SetActive(true);
        transform.position = PlayerManager.Instance.BodyPosition;
        hasLeftPlayer = false;
    }

    public override void DeactivateController()
    {
        gameObject.SetActive(false);
        hasLeftPlayer = false;
    }

    override protected void Awake()
    {
        base.Awake();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsActive) return;

        var enemy = collision.gameObject.GetComponent<GhostEnemy>();
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

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!IsActive) return;

        var enemy = collision.gameObject.GetComponent<GhostEnemy>();
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
        if (!IsActive) return;

        if (hasLeftPlayer)
        {
            var bodyController = collision.gameObject.GetComponent<BodyPlayerController>();
            if (bodyController)
            {
                PlayerManager.Instance.SetActiveController(bodyController);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!IsActive) return;

        var bodyController = collision.gameObject.GetComponent<BodyPlayerController>();
        if (bodyController)
        {
            hasLeftPlayer = true;
        }
    }
}
