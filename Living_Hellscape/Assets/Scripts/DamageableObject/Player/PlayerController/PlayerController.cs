using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerController : DamageableObject
{
    const float PRESENT_ITEM_TIME = 1f;

    //these two floats are temp, will get moved to a stats class or something
    [SerializeField]
    protected PlayerStats playerStats;

    [SerializeField]
    protected Transform heldObjectRoot;

    [SerializeField]
    PlayerInventory inventory;

    protected Vector2 lastDirection = Vector2.down;
   
    protected Rigidbody2D rb;
    protected BoxCollider2D boxCollider;

    protected InteractableObject interactableObject;

    public bool HasHeldObject => heldObjectRoot.childCount > 0;


    public bool IsActive => PlayerManager.Instance.Active == this;

    public PlayerStats PlayerStats => playerStats;

    public PlayerInventory Inventory => inventory;

    public Vector2 Velocity { get; protected set; }

    public abstract void ActivateController();

    public abstract void DeactivateController();

    override protected void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        playerStats.Setup();
        inventory.InstantiateInventory();
    }

    public void ControllerUpdate()
    {
        //if this is not the active controller return
        if (!IsActive) return;

        //poll user input, we use here in update, but b/c it belongs to the static inputcontroller class
        //it will be available in fixed with that correct values
        UserInput userInput = InputController.GetUserInput();

        if (HandlePauseAndInventory(userInput)) return;

        if(!(IsStunned() || IsScared()))
        {
            SetDirection(userInput.movement);
            Transform(userInput.transform);
            MainAction(userInput.mainAction);
            SecondAction(userInput.secondaryAction);
        }
    }

    public void ControllerFixedUpdate()
    {
        if (!IsActive) return;

        TestInteractableObject();

        UserInput userInput = InputController.GetUserInput();

        Move(userInput.movement);

        RotateEquip();
    }


    public Vector2 ThrowObject()
    {
        if (!heldObjectRoot) return Vector2.zero;
        if (!HasHeldObject) return Vector2.zero;
        interactableObject = null;
        RoomController.Instance.ActiveRoom.AllowRoomTransitions();
        return lastDirection;
    }

    public bool HoldObject(HoldableObject holdableObject)
    {
        if (!heldObjectRoot) return false;
        if (HasHeldObject) return false;

        holdableObject.transform.SetParent(heldObjectRoot, false);
        holdableObject.transform.position = heldObjectRoot.position;
        RoomController.Instance.ActiveRoom.StopRoomTransitions();

        return true;
    }

    public IEnumerator PresentItem(Item item)
    {
        GameController.Instance.SetStopUpdates(true);
        item.transform.SetParent(heldObjectRoot, false);
        item.transform.position = heldObjectRoot.position;
        item.transform.rotation = Quaternion.identity;

        item.StartPresent();
        
        float duration = PRESENT_ITEM_TIME;
        while(duration > 0)
        {
            duration -= Time.deltaTime;
            yield return null;
        }

        item.StopPresent();
        GameController.Instance.SetStopUpdates(false);
        inventory.EndAddItem(item);
    }

    public void SetTarget(Vector3 target)
    {
        StartCoroutine(TransitionRoom(target));
    }

    public IEnumerator StopControlForTime(float time)
    {
        PlayerManager.Instance.SetPlayerControl(false);

        while(time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        PlayerManager.Instance.SetPlayerControl(true);
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

    void TestInteractableObject()
    {
        if (interactableObject)
        {
            if (HasHeldObject)
            {
                //todo
            }
            else if (!rb.IsTouching(interactableObject.InteractableCollider))
            {
                interactableObject = null;
            }
        }
    }

    void MainAction(ButtonState buttonState)
    {
        if (HasHeldObject) return;
        if(buttonState == ButtonState.Down)
        {
            PlayerManager.Instance.Inventory.DoMainAction();
        }
    }

    void SecondAction(ButtonState buttonState)
    {
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
            GameController.Instance.SwitchWorlds();
            //PlayerManager.Instance.SwapActiveController();
        }
    }

    void Move(Vector2 movement)
    {
        var normInput = movement.normalized;

        Vector2 velocity = normInput * playerStats.Speed * Time.fixedDeltaTime;

        if (IsScared())
        {
            var scare = (Scare)GetStatusOfType(StatusEffectType.Scare);
            if ( scare != null )
            {
                velocity = scare.Vector * playerStats.Speed * Time.fixedDeltaTime;
            }
        }

        if (IsStunned())
        {
            velocity = Vector2.zero;
        }

        if(IsTakingDamage)
        {
            velocity += MoveByDamage();
        }

        Velocity = velocity / Time.fixedDeltaTime;

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
        PlayerManager.Instance.SetPlayerControl(false);
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
        PlayerManager.Instance.SetPlayerControl(true);
        GameController.Instance.EndRoomTransition();
    }
}
