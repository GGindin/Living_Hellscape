using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFireAttack : MonoBehaviour, IStatuser
{
    [SerializeField]
    Damage damage;

    [SerializeField]
    LayerMask playerBodyLayer;

    [SerializeField]
    float speed;

    [SerializeField]
    float lifeTime;

    [SerializeField]
    SpriteRenderer spriteRenderer;

    float startTime;

    HashSet<DamageableObject> hitObjects = new HashSet<DamageableObject>();

    public Vector2 Direction { get; set; }

    private void Awake()
    {
        startTime = Time.time;
    }

    public StatusEffect GetStatus(DamageableObject recievingObject)
    {
        if (!hitObjects.Contains(recievingObject))
        {
            if ((playerBodyLayer & 1 << recievingObject.gameObject.layer) != 0)
            {
                hitObjects.Add(recievingObject);
                return new Damage(damage);
            }
        }

        return null;
    }

    void Update()
    {
        CheckLifeTime();
    }

    private void FixedUpdate()
    {
        if (GameController.Instance.StopUpdates) return;
        Vector2 velocity = Direction * speed * Time.fixedDeltaTime;
        transform.position += new Vector3(velocity.x, velocity.y);
    }

    private void CheckLifeTime()
    {
        if (Time.time - startTime > lifeTime)
        {
            Destroy(gameObject);
        }

        float t = 1 - Mathf.InverseLerp(0f, lifeTime, Time.time - startTime);
        var col = spriteRenderer.color;
        col.a = t;
        spriteRenderer.color = col;
    }
}
