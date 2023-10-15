using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : DamageableObject
{
    [SerializeField]
    float health;

    [SerializeField]
    float speed;

    [SerializeField]
    float directionTime;

    [SerializeField]
    LayerMask levelLayer;

    [SerializeField]
    Damage damage;

    Rigidbody2D rb;

    float currentTimer = float.MaxValue;
    Vector2 direction;

    public bool isTakingDamage => damageFromOther != null;

    public Damage Damage => new Damage(damage);

    override protected void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
    }


    public void RoomUpdate() { }

    public void RoomFixedUpdate()
    {
        rb.velocity = Vector2.zero;

        UpdateDirection();

        Move();
    }

    protected override bool CheckHealthForDead()
    {
        return health <= 0f;
    }

    protected override void ChangeHealth(int delta)
    {
        health += delta;
    }

    private void Move()
    {
        Vector2 velocity = direction * speed * Time.fixedDeltaTime;

        if (damageFromOther != null)
        {
            velocity = MoveByDamage();
        }

        rb.MovePosition(rb.position + velocity);
    }

    void UpdateDirection()
    {
        if(currentTimer >= directionTime)
        {
            currentTimer = 0f;
            SetDirection();
        }
        else
        {
            currentTimer += Time.fixedDeltaTime;
        }
    }

    void SetDirection()
    {
        float angle = Random.Range(0f, 360f);
        direction = Quaternion.AngleAxis(angle, Vector3.forward) * Vector2.right;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var obj = collision.gameObject;
        if((levelLayer & 1 << obj.layer) != 0)
        {
            var contacts = collision.contacts;

            direction = Vector2.zero;

            foreach(var contact in contacts)
            {
                direction += contact.normal;
            }

            currentTimer = 0f;
            direction.Normalize();
            direction = Quaternion.Euler(0f, 0f, Random.Range(-30f, 30f)) * direction;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CollisionRouter collisionRouter = collision.gameObject.GetComponent<CollisionRouter>();

        if (collisionRouter && damageFromOther == null)
        {
            var target = collisionRouter.Target;
            Sword sword = target.GetComponent<Sword>();
            if (sword)
            {
                Vector2 damageDir = (rb.position - collision.ClosestPoint(rb.position)).normalized;
                var damage = sword.Damage;
                SetupDamage(damage, damageDir);
            }
        }
    }
}
