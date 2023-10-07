using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
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

    Animator animator;
    int hitID = Animator.StringToHash("hit");

    float currentTimer = float.MaxValue;
    Vector2 direction;

    Damage damageFromOther;

    public Damage Damage => new Damage(damage);

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        rb.velocity = Vector2.zero;

        UpdateDirection();
        if(damageFromOther == null)
        {
            Move();
        }
        else
        {
            MoveByDamage();
        }
    }

    private void Move()
    {
        rb.position += direction * speed * Time.fixedDeltaTime;
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

    void TakeDamage(Damage damage)
    {
        damageFromOther = damage;

        health -= damageFromOther.amount;

        if(health <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        animator.SetBool(hitID, true);
    }

    private void MoveByDamage()
    {       
        float t = Mathf.InverseLerp(0, damageFromOther.Duration, damageFromOther.CurrentTime);
        Vector2 offset = Vector2.Lerp(Vector2.zero, damageFromOther.Vector, t) * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + offset);
        damageFromOther.CurrentTime -= Time.fixedDeltaTime;
        if (t <= 0)
        {
            damageFromOther = null;
            animator.SetBool(hitID, false);
        }
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
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CollisionRouter collisionRouter = collision.gameObject.GetComponent<CollisionRouter>();

        if (collisionRouter)
        {
            var target = collisionRouter.Target;
            Sword sword = target.GetComponent<Sword>();
            if (sword)
            {
                Vector2 damageDir = (rb.position - collision.ClosestPoint(rb.position)).normalized;
                var damage = sword.Damage;
                damage.SetVectorFromDirection(damageDir);

                TakeDamage(damage);
            }
        }
    }
}
