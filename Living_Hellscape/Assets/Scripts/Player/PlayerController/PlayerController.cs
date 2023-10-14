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
   
    protected Rigidbody2D rb;
    protected BoxCollider2D boxCollider;
    
    protected Animator animator;
    protected int hitID = Animator.StringToHash("hit");

    protected Damage damageFromOther;

    public bool isActive = false;

    bool hasControl = true;

    public bool HasControl => hasControl;

    public abstract void ActivateController();

    public abstract void DeactivateController();

    virtual protected void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();    
    }

    // Update is called once per frame
    void Update()
    {
        //if this is not the active controller return
        if (!isActive) return;

        //poll user input, we use here in update, but b/c it belongs to the static inputcontroller class
        //it will be available in fixed with that correct values
        UserInput userInput = InputController.GetUserInput();

        if (userInput.pause == ButtonState.Down)
        {
            //pause game TODO
        }

        if (hasControl)
        {
            SetDirection(userInput.movement);
            Transform(userInput.transform);
            MainAction(userInput.mainAction);
            SecondAction(userInput.secondaryAction);
        }
    }



    void FixedUpdate()
    {
        if (!isActive) return;

        if (hasControl)
        {
            UserInput userInput = InputController.GetUserInput();

            Move(userInput.movement);

            RotateEquip();
        }
    }

    public void SetTarget(Vector3 target)
    {
        StartCoroutine(TransitionRoom(target));
    }

    void MainAction(ButtonState buttonState)
    {
        if(buttonState == ButtonState.Down)
        {
            if (equip)
            {
                equip.TriggerAction();
            }
        }
    }

    void SecondAction(ButtonState buttonState)
    {
        //is no second action, will be interactions and second item
    }

    void Transform(ButtonState buttonState)
    {
        if(this is BodyPlayerController && buttonState == ButtonState.Down)
        {
            PlayerManager.instance.SwapActiveController();
        }
    }

    void Move(Vector2 movement)
    {
        var normInput = movement.normalized;

        Vector2 velocity = normInput * speed * Time.fixedDeltaTime;

        if(damageFromOther != null)
        {
            velocity += MoveByDamage();
        }

        rb.MovePosition(rb.position + velocity);
    }

    private void SetDirection(Vector2 movement)
    {     
        //if x is greater than y
        if(Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
        {
            lastDirection.x = 1f * Mathf.Sign(movement.x);
            lastDirection.y = 0f;
        }
        //if y is greater than x
        else if(Mathf.Abs(movement.x) < Mathf.Abs(movement.y))
        {
            lastDirection.y = 1f * Mathf.Sign(movement.y);
            lastDirection.x = 0f;
        }
    }

    void RotateEquip()
    {
        if (equip)
        {
            equip.SetDirection(lastDirection);
        }      
    }

    private Vector2 MoveByDamage()
    {      
        float t = Mathf.InverseLerp(0, damageFromOther.Duration, damageFromOther.CurrentTime);
        Vector2 offset = Vector2.Lerp(Vector2.zero, damageFromOther.Vector, t) * Time.fixedDeltaTime;
        //rb.MovePosition(rb.position + offset);
        damageFromOther.CurrentTime -= Time.fixedDeltaTime;

        if (t <= 0)
        {
            damageFromOther = null;
            animator.SetBool(hitID, false);
            if (health <= 0f)
            {
                Destroy(gameObject);
                return Vector2.zero;
            }
        }

        return offset;
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
