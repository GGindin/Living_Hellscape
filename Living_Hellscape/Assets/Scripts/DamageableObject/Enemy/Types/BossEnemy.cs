using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : EnemyController
{
    PlayerController playerController;

    [SerializeField]
    float behaviorUpdateTime;

    [SerializeField]
    float wanderDirectionTime;

    [SerializeField, Range(0f, 1f)]
    float behaviorChangeProb;

    [SerializeField]
    string enterText;

    [SerializeField]
    string exitText;

    float currentWanderDirectionTime = float.MaxValue;

    float currentBehaviorTime = float.MaxValue;

    bool behaviorIsWander;

    bool hasDoneFirstUpdate;

    protected override void Awake()
    {
        base.Awake();
        currentSpeed = speed;
    }

    public override void RoomUpdate() { }

    public override void RoomFixedUpdate()
    {
        if (!hasDoneFirstUpdate)
        {
            hasDoneFirstUpdate = true;
            if(enterText != null)
            {
                TextBoxController.instance.OpenTextBox(enterText);
            }
        }

        rb.velocity = Vector2.zero;

        UpdateBehavior();

        UpdateTarget();

        Move();
    }

    private void UpdateBehavior()
    {
        currentBehaviorTime += Time.deltaTime;

        if(currentBehaviorTime > behaviorUpdateTime)
        {
            currentBehaviorTime = 0f;
            if (Random.Range(0f, 1f) < behaviorChangeProb)
            {
                if (behaviorIsWander) SetBehaviorToChase();
                else
                {
                    SetBehaviorToWander();
                }
            }
        }
        else
        {
            if (behaviorIsWander)
            {
                currentWanderDirectionTime += Time.deltaTime;
            }
        }
    }

    void UpdateTarget()
    {
        if (behaviorIsWander)
        {
            currentWanderDirectionTime += Time.deltaTime;
            if(currentWanderDirectionTime > wanderDirectionTime)
            {
                SetBehaviorToWander();
            }
        }
        else
        {
            FindPlayerController();
            SetChaseDirection();
        }
    }

    void FindPlayerController()
    {
        var controller = PlayerManager.Instance.Active;
        if (controller is BodyPlayerController)
        {
            playerController = (BodyPlayerController)controller;
            return;
        }
        else
        {
            playerController = null;
        }
    }

    void SetChaseDirection()
    {
        if (playerController)
        {
            direction = (playerController.transform.localPosition - transform.localPosition).normalized;
        }
        else
        {
            SetBehaviorToWander();
        }
    }

    void SetBehaviorToChase()
    {
        behaviorIsWander = false;
    }

    void SetBehaviorToWander()
    {
        behaviorIsWander = true;
        currentWanderDirectionTime = 0f;
        SetWanderDirection();
    }

    //wanderer

    void SetWanderDirection()
    {
        float angle = Random.Range(0f, 360f);
        direction = Quaternion.AngleAxis(angle, Vector3.forward) * Vector2.right;
    }

    protected override void HitLayerReset()
    {
        if (behaviorIsWander)
        {
            SetBehaviorToWander();
        }
    }


    public override void LoadPerm(GameDataReader reader)
    {

    }

    public override void LoadTemp(GameDataReader reader)
    {

    }

    public override void SavePerm(GameDataWriter writer)
    {
        writer.WriteInt(1);
    }

    public override void SaveTemp(GameDataWriter writer)
    {

    }

    private void OnDestroy()
    {
        if(exitText != null && health <= 0)
        {
            TextBoxController.instance.OpenTextBox(exitText);
        }
    }
}
