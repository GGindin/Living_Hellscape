using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerController : DamageableObject
{
    //these two floats are temp, will get moved to a stats class or something
    [SerializeField]
    protected PlayerStats playerStats;

    [SerializeField]
    protected Transform heldObjectRoot;

    protected Vector2 lastDirection = Vector2.down;
   
    protected Rigidbody2D rb;
    protected BoxCollider2D boxCollider;

    public bool isActive = false;

    protected InteractableObject interactableObject;

    bool hasControl = true;

    public bool HasControl => hasControl;

    public PlayerStats PlayerStats => playerStats;

    public abstract void ActivateController();

    public abstract void DeactivateController();

    override protected void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        playerStats.Setup();
    }

    // Update is called once per frame
    void Update()
    {
        //if this is not the active controller return
        if (!isActive) return;

        //poll user input, we use here in update, but b/c it belongs to the static inputcontroller class
        //it will be available in fixed with that correct values
        UserInput userInput = InputController.GetUserInput();

        if (HandlePauseAndInventory(userInput)) return;

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

        if (interactableObject)
        {
            if (!rb.IsTouching(interactableObject.InteractableCollider))
            {
                interactableObject = null;
            }
        }

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

    bool HandlePauseAndInventory(UserInput userInput)
    {
        if (userInput.pause == ButtonState.Down)
        {
            //pause game TODO
            GameController.Instance.SetPause();
            GameController.Instance.HandlePause();
        }
        else if (userInput.inventory == ButtonState.Down)
        {
            GameController.Instance.SetPause();
            PlayerManager.Instance.HandleInventory();
        }

        if (GameController.Instance.Paused)
        {
            return true;
        }

        return false;
    }

    void MainAction(ButtonState buttonState)
    {
        if(buttonState == ButtonState.Down)
        {
            PlayerManager.Instance.Inventory.DoMainAction();
        }
    }

    void SecondAction(ButtonState buttonState)
    {
        //currently just does the second action if set, will setup so that if we are touching an interactable we will interact
        //instead
        if (buttonState == ButtonState.Down)
        {
            if (interactableObject)
            {
                interactableObject.Interact();
            }
            else
            {
                PlayerManager.Instance.Inventory.DoSecondAction();
            }         
        }
    }

    void Transform(ButtonState buttonState)
    {
        if(this is BodyPlayerController && buttonState == ButtonState.Down)
        {
            PlayerManager.Instance.SwapActiveController();
        }
    }

    void Move(Vector2 movement)
    {
        var normInput = movement.normalized;

        Vector2 velocity = normInput * playerStats.Speed * Time.fixedDeltaTime;

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
        PlayerManager.Instance.Inventory.RotateEquipment(lastDirection);     
    }

    protected override bool CheckHealthForDead()
    {
        return playerStats.CurrentHealth <= 0f;
    }

    protected override void ChangeHealth(int delta)
    {
        playerStats.ChangeCurrentHealth(delta);
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
            rb.MovePosition(Vector3.Lerp(startPos, target, t));
            yield return null;
        }

        rb.MovePosition(target);
        hasControl = true;
        GameController.Instance.EndRoomTransition();
    }
}
