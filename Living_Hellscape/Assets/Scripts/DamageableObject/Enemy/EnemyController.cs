using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyController : DamageableObject, ISaveableObject
{
    //some of these will get put into an enemy stats struct at some point
    [SerializeField]
    protected float health;

    [SerializeField]
    protected float speed;

    [SerializeField]
    protected Damage damage;

    [SerializeField]
    protected LayerMask redirectLayers;

    protected Rigidbody2D rb;

    protected Vector2 direction;

    protected float currentSpeed;

    protected Vector3 startingPos;

    public Damage Damage => new Damage(damage);

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        startingPos = transform.localPosition;
    }


    public abstract void RoomUpdate();

    public abstract void RoomFixedUpdate();

    protected override bool CheckHealthForDead()
    {
        return health <= 0f;
    }

    protected override void ChangeHealth(int delta)
    {
        health += delta;
    }

    protected void Move()
    {
        Vector2 velocity = direction * currentSpeed * Time.fixedDeltaTime;

        if (IsTakingDamage)
        {
            velocity = MoveByDamage();
        }

        rb.MovePosition(rb.position + velocity);
    }

    protected abstract void HitLayerReset();

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var obj = collision.gameObject;
        if((redirectLayers & 1 << obj.layer) != 0)
        {
            HitLayerReset();

            var contacts = collision.contacts;

            direction = Vector2.zero;

            foreach(var contact in contacts)
            {
                direction += contact.normal;
            }

            direction.Normalize();
            direction = Quaternion.Euler(0f, 0f, Random.Range(-30f, 30f)) * direction;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DamageRouter damageRouter = collision.gameObject.GetComponent<DamageRouter>();

        if (damageRouter)
        {
            var target = damageRouter.Target;
            if (target)
            {
                Vector2 damageDir = (rb.position - collision.ClosestPoint(rb.position)).normalized;
                var damage = target.Damager.GetDamage();
                AddStatusEffect(damage, damageDir);
            }
        }
    }

    public string GetFileName()
    {
        //this should never get called on an enemy
        throw new System.NotImplementedException();
    }

    public abstract void SavePerm(GameDataWriter writer);

    public abstract void LoadPerm(GameDataReader reader);

    public abstract void SaveTemp(GameDataWriter writer);

    public abstract void LoadTemp(GameDataReader reader);
}
