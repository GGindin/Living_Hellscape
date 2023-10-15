using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    Damage damage;

    [SerializeField]
    float speed;

    [SerializeField]
    LayerMask hitMask;

    Rigidbody2D body;

    public Vector2 Direction { get; set; }

    public Damage Damage => new Damage(damage);

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
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

            var damage = Damage;

            damageable.SetupDamage(damage, -damageDir.normalized);
        }

        Destroy(gameObject);
    }
}
