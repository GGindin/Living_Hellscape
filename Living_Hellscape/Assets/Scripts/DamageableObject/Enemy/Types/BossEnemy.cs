using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : EnemyController
{
    PlayerController playerController;

    [SerializeField]
    BossRingAttack ringAttackPrefab;

    [SerializeField]
    BossFireAttack fireAttackPrefab;

    [SerializeField]
    Transform fireAttackStartPos;

    [SerializeField]
    float behaviorUpdateTime;

    [SerializeField]
    float wanderDirectionTime;

    [SerializeField, Range(0f, 1f)]
    float chaseBehaviorChangeProb;

    [SerializeField]
    BossRingAttackSettings ringAttackSettings;

    [SerializeField]
    BossFireAttackSettings fireAttackSettings;

    [SerializeField]
    float attackStopTime;

    [SerializeField]
    string enterText;

    [SerializeField]
    string exitText;

    float currentWanderDirectionTime = float.MaxValue;

    float currentBehaviorTime = float.MaxValue;

    float currentAttackStopTime = float.MaxValue;

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

        Attack();

        UpdateBehavior();

        UpdateTarget();

        if(currentAttackStopTime > attackStopTime)
        {
            Move();
        }     
    }

    private void Attack()
    {
        if (playerController)
        {
            if (fireAttackSettings.CurrentCoolDown > fireAttackSettings.coolDown)
            {
                fireAttackSettings.CurrentCoolDown = 0f;
                if (Random.Range(0f, 1f) < fireAttackSettings.probablity)
                {
                    currentAttackStopTime = 0f;
                    var fireAttack = Instantiate(fireAttackPrefab, transform.position + Vector3.up * .5f, Quaternion.LookRotation(Vector3.forward, direction));
                    fireAttack.transform.SetParent(RoomController.Instance.ActiveRoom.DynamicObjectsHolder, true);
                    fireAttack.Direction = (playerController.transform.position - transform.position).normalized;
                }

            }
        }

        if (ringAttackSettings.CurrentCoolDown > ringAttackSettings.coolDown)
        {
            ringAttackSettings.CurrentCoolDown = 0f;
            if (Random.Range(0f, 1f) < ringAttackSettings.probablity)
            {
                currentAttackStopTime = 0f;

                Instantiate(ringAttackPrefab, transform.position, Quaternion.identity, RoomController.Instance.ActiveRoom.DynamicObjectsHolder);
            }

        }

        if (ringAttackSettings.CurrentCoolDown < ringAttackSettings.coolDown)
        {
            ringAttackSettings.CurrentCoolDown += Time.deltaTime;
        }

        if (fireAttackSettings.CurrentCoolDown < fireAttackSettings.coolDown)
        {
            fireAttackSettings.CurrentCoolDown += Time.deltaTime;
        }

        if (currentAttackStopTime < attackStopTime)
        {
            currentAttackStopTime += Time.deltaTime;
        }
    }

    private void UpdateBehavior()
    {
        currentBehaviorTime += Time.deltaTime;

        if(currentBehaviorTime > behaviorUpdateTime)
        {
            currentBehaviorTime = 0f;
            if (behaviorIsWander) SetBehaviorToChase();
            else
            {
                if (Random.Range(0f, 1f) < chaseBehaviorChangeProb)
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
        SetBehaviorToWander();
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

[System.Serializable]
public struct BossRingAttackSettings
{
    public float probablity;
    public float coolDown;
    
    public float CurrentCoolDown { get; set; }
}

[System.Serializable]
public struct BossFireAttackSettings
{
    public float probablity;
    public float coolDown;

    public float CurrentCoolDown { get; set; }
}
