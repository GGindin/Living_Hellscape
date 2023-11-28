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
    LayerMask thrownItemLayer;

    [SerializeField]
    LayerMask heldItemLayer;

    [SerializeField]
    GameObject shadow;

    SpriteRenderer spriteRenderer;

    public override Collider2D InteractableCollider => objectCollider;

    public override SpriteRenderer SpriteRenderer => spriteRenderer;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        objectCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void Interact()
    {
        if (!isHeld)
        {
            AudioController.Instance.PlaySoundEffect("pickupbox");
            isHeld = PlayerManager.Instance.Active.HoldObject(this);
            if (isHeld)
            {
                gameObject.layer = LayerUtil.LayerMaskToLayer(heldItemLayer);
                spriteRenderer.sortingOrder = 1;
            }       
        }
        else if (!isThrown)
        {
            isHeld = false;
            var direction = PlayerManager.Instance.Active.ThrowObject();
            transform.SetParent(RoomController.Instance.ActiveRoom.transform);
            gameObject.layer = LayerUtil.LayerMaskToLayer(thrownItemLayer);
            StartCoroutine(Throw(direction));
        }
    }

    IEnumerator Throw(Vector2 direction)
    {
        isThrown = true;
        body.isKinematic = false;

        shadow.SetActive(true);

        var playerTrans = PlayerManager.Instance.Active.transform;
        var playerPos = new Vector2(playerTrans.position.x, playerTrans.position.y);
        var end = playerPos + (direction * throwDistance);
        var currentThrowDistance = throwDistance;

        var shadowStartPos = playerPos + Vector2.down * .5f;
        var shadowEndPos = shadowStartPos + (direction * throwDistance);
        shadow.transform.position = shadowStartPos;

        var hitResult = Physics2D.Raycast(playerPos, direction, throwDistance, geometryLayer);
        if(hitResult)
        {
            end = hitResult.point;
            currentThrowDistance = (end - playerPos).magnitude;
            shadowEndPos = shadowStartPos + (direction * currentThrowDistance);
        }

        AudioController.Instance.PlaySoundEffect("throw");
        PlayerManager.Instance.Active.StartCoroutine(PlayerManager.Instance.Active.StopControlForTime(.25f));

        var generalThrowSpeed = throwDistance / throwTime;
        var distanceRatio = throwDistance / currentThrowDistance;
        var throwSpeed = generalThrowSpeed * distanceRatio;
        //throwSpeed += Vector2.Dot(direction, PlayerManager.Instance.Active.Velocity);

        var actualThrowTime = throwTime / distanceRatio;
        //var actualThrowTime = currentThrowDistance / throwSpeed;
        //var actualThrowTime = throwTime;




        var start = body.position;

        float currentTime = 0f;

        while(currentTime < actualThrowTime)
        {
            currentTime += Time.deltaTime;
            float t = Mathf.InverseLerp(0f, actualThrowTime, currentTime);
            var pos = Vector2.Lerp(start, end, t);
            body.MovePosition(pos);

            shadow.transform.position = Vector2.Lerp(shadowStartPos, shadowEndPos, t);
            yield return null;
        }

        body.MovePosition(end);

        AudioController.Instance.PlaySoundEffect("box");
        ParticleSystemController.Instance.AddHoldableBreak(transform.position);

        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isThrown) return;

        AudioController.Instance.PlaySoundEffect("box");
        ParticleSystemController.Instance.AddHoldableBreak(transform.position);

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
            damageable.AddStatusEffect(newDamage, direction.normalized);
            
        }
        Destroy(gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!isThrown) return;

        AudioController.Instance.PlaySoundEffect("box");
        ParticleSystemController.Instance.AddHoldableBreak(transform.position);



        var damageable = collision.gameObject.GetComponent<DamageableObject>();

        if (damageable)
        {
            var contacts = collision.contacts;
            var direction = new Vector2();

            foreach (var contact in contacts)
            {
                direction += contact.normal;
            }

            direction *= -1;

            var newDamage = new Damage(damage);
            damageable.AddStatusEffect(newDamage, direction.normalized);
            
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var statusRouter = collision.gameObject.GetComponent<StatusRouter>();
        if (statusRouter)
        {
            var sword = statusRouter.Target.gameObject.GetComponent<Sword>();
            if (sword != null)
            {
                AudioController.Instance.PlaySoundEffect("box");
                ParticleSystemController.Instance.AddHoldableBreak(transform.position);

                Destroy(gameObject);
            }
        }

    }

    public override string GetFileName()
    {
        //never should get called
        throw new System.NotImplementedException();
    }

    public override void SavePerm(GameDataWriter writer)
    {
        //never should get called
        throw new System.NotImplementedException();
    }

    public override void LoadPerm(GameDataReader reader)
    {
        //never should get called
        throw new System.NotImplementedException();
    }
    public override void SaveTemp(GameDataWriter writer)
    {
        writer.WriteInt(1);
    }

    //doesn't really have anything to load
    //and the room will read the exists write
    public override void LoadTemp(GameDataReader reader) { }


}
