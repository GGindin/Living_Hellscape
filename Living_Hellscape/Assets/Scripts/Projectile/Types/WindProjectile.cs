using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindProjectile : MonoBehaviour, IStatuser
{
    [SerializeField]
    LayerMask EnemyBodyLayer;

    [SerializeField]
    LayerMask EnemyGhostLayer;

    [SerializeField]
    float speed;

    [SerializeField]
    float lifeTime;

    [SerializeField]
    SpriteRenderer spriteRenderer;

    float startTime;

    Scare scare;

    Stun stun;

    HashSet<DamageableObject> hitObjects = new HashSet<DamageableObject>();   

    public Vector2 Direction { get; set; }

    public StatusEffect GetStatus(DamageableObject recievingObject)
    {
        if (!hitObjects.Contains(recievingObject))
        {
            if ((EnemyBodyLayer & 1 << recievingObject.gameObject.layer) != 0)
            {
                hitObjects.Add(recievingObject);
                return new Scare(scare);
            }
            else if ((EnemyGhostLayer & 1 << recievingObject.gameObject.layer) != 0)
            {
                hitObjects.Add(recievingObject);
                return new Stun(stun);
            }
        }

        return null;
    }

    public void SetScare(Scare scare)
    {
        this.scare = scare;
        startTime = Time.time;
    }

    public void SetStun(Stun stun)
    {
        this.stun = stun;
        startTime = Time.time;
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
        if(Time.time - startTime > lifeTime)
        {
            Destroy(gameObject);
        }

        float t = 1 - Mathf.InverseLerp(0f, lifeTime, Time.time - startTime);
        var col = spriteRenderer.color;
        col.a = t;
        spriteRenderer.color = col;
    }
}
