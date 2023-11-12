using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRingAttack : MonoBehaviour, IStatuser
{
    [SerializeField]
    Stun stun;

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

    private void Awake()
    {
        startTime = Time.time;
    }

    public Vector2 Direction { get; set; }

    public StatusEffect GetStatus(DamageableObject recievingObject)
    {
        if (!hitObjects.Contains(recievingObject))
        {
            if ((playerBodyLayer & 1 << recievingObject.gameObject.layer) != 0)
            {
                hitObjects.Add(recievingObject);
                return new Stun(stun);
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
        transform.localScale += Vector3.one * speed * Time.deltaTime;
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
