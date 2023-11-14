using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [SerializeField]
    protected DroppableObjectTable enemyDropTable;

    protected Rigidbody2D rb;

    protected SpriteRenderer spriteRenderer;

    protected Vector2 direction;

    protected float currentSpeed;

    protected Vector3 startingPos;

    public Damage Damage => new Damage(damage);

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        startingPos = transform.position;
    }

    public abstract void RoomUpdate();

    public abstract void RoomFixedUpdate();

    protected override void DestroyObject()
    {
        Destroy(gameObject);
    }

    protected override bool CheckHealthForDead()
    {
        bool isDead = health <= 0f;
        if (isDead)
        {
            DropDrops();
        }

        return isDead;
    }

    protected override void ChangeHealth(int delta)
    {
        health += delta;
        if (delta < 0f)
        {
            AudioController.Instance.PlaySoundEffect("box");
        }
        if (health <= 0f)
        {
            AudioController.Instance.PlaySoundEffect("death");
        }
    }

    protected void Move()
    {
        Vector2 velocity = direction * currentSpeed * Time.fixedDeltaTime;

        if (IsScared())
        {
            velocity = GetScaredDirection() * currentSpeed * Time.fixedDeltaTime;
        }

        if (IsStunned())
        {
            velocity = Vector2.zero;
        }

        if (IsTakingDamage)
        {
            velocity = MoveByDamage();
        }

        SetAnimatorDirection(velocity.normalized);


        rb.MovePosition(rb.position + velocity);
    }

    protected virtual void SetAnimatorDirection(Vector3 direction)
    {
    }

    protected abstract void HitLayerReset(Collision2D collision);

    private Vector2 GetScaredDirection()
    {
        var scare = (Scare)GetStatusOfType(StatusEffectType.Scare);

        if (scare == null) return Vector2.zero;

        var dir = scare.Vector;
        var vec3 = new Vector3(dir.x, dir.y);

        vec3 = (transform.localPosition - startingPos).normalized + vec3;
        return vec3.normalized;
    }

    protected void DropDrops()
    {
        if (enemyDropTable == null) return;

        List<DroppableObject> drops = (List<DroppableObject>)enemyDropTable.Result();
        var dropCount = drops.Count(d => !d.IsNull);
        var rotIter = 360f / dropCount;
        Vector2 vec = Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.forward) * Vector3.up;
        for(int i = 0; i < drops.Count; i++)
        {
            var drop = drops[i];

            if (drop.IsNull) continue;

            var enemyDrop = drop as EnemyDroppable;
            var itemDrop = enemyDrop.GetItemDropInstance();
            itemDrop.transform.SetParent(RoomController.Instance.ActiveRoom.DynamicObjectsHolder);
            itemDrop.transform.position = transform.position;
            itemDrop.StartCoroutine(itemDrop.Drop(Quaternion.AngleAxis(rotIter * i, Vector3.forward) * vec));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var obj = collision.gameObject;
        if((redirectLayers & 1 << obj.layer) != 0)
        {
            HitLayerReset(collision);

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
        StatusRouter statusRouter = collision.gameObject.GetComponent<StatusRouter>();

        if (statusRouter)
        {
            var target = statusRouter.Target;
            if (target)
            {
                Vector2 damageDir = (rb.position - collision.ClosestPoint(rb.position)).normalized;
                var status = target.Statuser.GetStatus(this);
                if(status != null)
                {
                    AddStatusEffect(status, damageDir);
                }
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
