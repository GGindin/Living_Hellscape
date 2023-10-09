using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerController : MonoBehaviour
{
    //these two floats are temp, will get moved to a stats class or something
    [SerializeField]
    protected float health;

    [SerializeField]
    protected float speed;

    //this is temporary until we get the inventory and EquipedGear setup
    [SerializeField]
    protected Equipment equip;

    protected Vector2 lastDirection = Vector2.down;
    protected Vector2 normInput = new Vector2();
   
    protected Rigidbody2D rb;
    protected BoxCollider2D boxCollider;
    
    protected Animator animator;
    protected int hitID = Animator.StringToHash("hit");

    protected Damage damageFromOther;

    public bool isActive = false;

    bool hasControl = true;

    public bool HasControl => hasControl;

    virtual protected void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();    
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive) return;

        normInput = GetNormInput();
        if (Input.GetKeyDown(KeyCode.K) && equip)
        {
            equip.TriggerAction();
        }
        if (hasControl && Input.GetKeyDown(KeyCode.Space))
        {
            //temporary while we split this code into the two classes
            PlayerManager.instance.SwapActiveController();
        }
        
    }

    void FixedUpdate()
    {
        if (!isActive) return;

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

    public abstract void ActivateController();

    public abstract void DeactivateController();

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

    void RotateEquip()
    {
        if (equip)
        {
            equip.SetDirection(lastDirection);
        }      
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

    protected void TakeDamage(Damage damage)
    {
        damageFromOther = damage;

        health -= damageFromOther.amount;

        animator.SetBool(hitID, true);
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
}
