using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandererEnemy : EnemyController
{
    [SerializeField]
    float directionTime;

    [SerializeField]
    float maxWanderDistance;

    float currentTimer = float.MaxValue;

    protected override void Awake()
    {
        base.Awake();
        currentSpeed = speed;
    }

    public override void RoomUpdate() { }

    public override void RoomFixedUpdate()
    {
        rb.velocity = Vector2.zero;

        UpdateDirection();

        Move();
    }

    void SetDirection()
    {
        float angle = Random.Range(0f, 360f);
        direction = Quaternion.AngleAxis(angle, Vector3.forward) * Vector2.right;
    }

    void UpdateDirection()
    {
        if (currentTimer >= directionTime)
        {
            currentTimer = 0f;
            SetDirection();
        }
        else if (IsPastMaxWanderDistance())
        {
            currentTimer = 0f;
            direction = startingPos - transform.position;
            direction.Normalize();
        }
        else
        {
            currentTimer += Time.fixedDeltaTime;
        }
    }

    bool IsPastMaxWanderDistance()
    {
        float sqrDst = (startingPos - transform.position).sqrMagnitude;
        return sqrDst > maxWanderDistance * maxWanderDistance;
    }

    protected override void HitLayerReset()
    {
        currentTimer = 0f;
    }

    public override void SavePerm(GameDataWriter writer) { }

    public override void LoadPerm(GameDataReader reader) { }

    public override void SaveTemp(GameDataWriter writer)
    {
        writer.WriteInt(1);
        writer.WriteFloat(health);
    }

    public override void LoadTemp(GameDataReader reader)
    {
        health = reader.ReadFloat();
    }


}
