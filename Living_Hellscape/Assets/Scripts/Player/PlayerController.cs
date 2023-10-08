using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float health;

    [SerializeField]
    float speed;

    [SerializeField]
    PlayerInventory inventory;

    [SerializeField]
    Equipment equip;

    Vector2 lastDirection = Vector2.down;
    Vector2 normInput = new Vector2();

    Rigidbody2D rb;
    
    Animator animator;
    int hitID = Animator.StringToHash("hit");

    bool hasControl = true;
    Damage damageFromOther;

    public bool HasControl => hasControl;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();    
    }

    private void Start()
    {
        GameController.instance.AssignPlayer(this);
    }

    // Update is called once per frame
    void Update()
    {
        normInput = GetNormInput();
        if (Input.GetKeyDown(KeyCode.K))
        {
            equip.TriggerAction();
        }
        
    }

    void FixedUpdate()
    {
        if (damageFromOther == null)
        {
            MoveByUserInput();
        }
        else
        {
            MoveByDamage();
        }
        
        RotateEquip();
    }

    public void SetTarget(Vector3 target)
    {
        StartCoroutine(TransitionRoom(target));
    }

    void MoveByUserInput()
    {
        if (hasControl)
        {
            Vector2 velocity = normInput * speed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + velocity);
        }

        if(normInput.sqrMagnitude > .1f)
        {
            lastDirection = normInput;
        }        
    }

    void TakeDamage(Damage damage)
    {
        damageFromOther = damage;

        health -= damageFromOther.amount;

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
            if (health <= 0f)
            {
                Destroy(gameObject);
                return;
            }
        }
    }

    void RotateEquip()
    {
        equip.SetDirection(lastDirection);
    }

    Vector2 GetNormInput()
    {
        if (hasControl)
        {
            Vector2 input = new Vector2();
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");
            input.Normalize();
            return input;
        }

        return Vector2.zero;
    }

    IEnumerator TransitionRoom(Vector3 target)
    {
        hasControl = false;
        Vector3 startPos = rb.position;

        float duration = 2f;
        float currentTime = 0f;

        while(currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float t = Mathf.InverseLerp(0f, duration, currentTime);
            rb.position = Vector3.Lerp(startPos, target, t);
            yield return null;
        }

        rb.position = target;
        hasControl = true;
        GameController.instance.EndRoomTransition();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var door = collision.gameObject.GetComponent<Door>();
        if (door)
        {
            door.OperateDoor();
            return;
        }

        var enemy = collision.gameObject.GetComponent<EnemyController>();
        if (enemy && damageFromOther == null && !enemy.isTakingDamage)
        {
            var contacts = collision.contacts;

            Vector2 damageDir = Vector2.zero;

            foreach (var contact in contacts)
            {
                damageDir += contact.normal;
            }

            var damage = enemy.Damage;
            damage.SetVectorFromDirection(damageDir.normalized);

            TakeDamage(damage);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var doorTrigger = collision.gameObject.GetComponent<DoorTrigger>();
        if (doorTrigger)
        {
            doorTrigger.SignalTrigger();
        }
    }
}
