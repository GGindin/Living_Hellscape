using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldableObject : InteractableObject
{
    Rigidbody2D body;

    Collider2D objectCollider;

    bool isHeld = false;
    bool isThrown = false;

    [SerializeField]
    Damage damage;

    [SerializeField]
    float throwTime;

    [SerializeField]
    float throwDistance;

    [SerializeField]
    LayerMask geometryLayer;

    [SerializeField]
    LayerMask itemLayer;

    public override Collider2D InteractableCollider => objectCollider;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        objectCollider = GetComponent<Collider2D>();
    }

    public override void Interact()
    {
        if (!isHeld)
        {
            isHeld = PlayerManager.Instance.Active.HoldObject(this);
            if (isHeld)
            {
                gameObject.layer = LayerUtil.LayerMaskToLayer(itemLayer);
            }       
        }
        else if (!isThrown)
        {
            isHeld = false;
            var direction = PlayerManager.Instance.Active.ThrowObject();
            transform.SetParent(RoomController.Instance.ActiveRoom.transform);
            StartCoroutine(Throw(direction));
        }
    }

    IEnumerator Throw(Vector2 direction)
    {
        isThrown = true;

        var throwSpeed = throwDistance / throwTime;
        throwSpeed += Vector2.Dot(direction, PlayerManager.Instance.Active.Velocity);

        var actualThrowTime = throwDistance / throwSpeed;

        var playerTrans = PlayerManager.Instance.Active.transform;
        var playerPos = new Vector2(playerTrans.position.x, playerTrans.position.y);
        var end = playerPos + (direction * throwDistance);

        var start = body.position;

        float currentTime = 0f;

        while(currentTime < actualThrowTime)
        {
            currentTime += Time.deltaTime;
            float t = Mathf.InverseLerp(0f, actualThrowTime, currentTime);
            var pos = Vector2.Lerp(start, end, t);
            body.MovePosition(pos);
            yield return null;
        }

        body.MovePosition(end);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isThrown) return;

        var damageable = collision.gameObject.GetComponent<DamageableObject>();

        if (damageable)
        {
            var contacts = collision.contacts;
            var direction = new Vector2();

            foreach( var contact in contacts)
            {
                direction += contact.normal;
            }

            direction *= -1;

            var newDamage = new Damage(damage);
            damageable.SetupDamage(newDamage, direction.normalized);
            Destroy(gameObject);
        }

    }
}