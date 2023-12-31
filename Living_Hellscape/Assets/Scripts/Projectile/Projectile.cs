using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    StatusEffect statusEffect;

    [SerializeField]
    float speed;

    [SerializeField]
    LayerMask hitMask;

    Rigidbody2D body;

    public Vector2 Direction { get; set; }


    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    public void SetDamage(StatusEffect statusEffect)
    {
        this.statusEffect = statusEffect;
    }

    private void FixedUpdate()
    {
        if (GameController.Instance.StopUpdates) return;
        Vector2 velocity = Direction * speed * Time.fixedDeltaTime;
        body.MovePosition(body.position + velocity);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var damageable = collision.gameObject.GetComponent<DamageableObject>();

        if (damageable != null)
        {
            var contacts = collision.contacts;

            Vector2 damageDir = Vector2.zero;

            foreach (var contact in contacts)
            {
                damageDir += contact.normal;
            }

            damageable.AddStatusEffect(statusEffect, -damageDir.normalized);
        }
        ParticleSystemController.Instance.AddMarbleBreak(transform.position);
        Destroy(gameObject);
    }
}
