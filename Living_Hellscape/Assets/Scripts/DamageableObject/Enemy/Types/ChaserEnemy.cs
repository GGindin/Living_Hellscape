using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaserEnemy : EnemyController
{
    PlayerController playerController;

    [SerializeField]
    float maxChaseDistance;

    protected override void Awake()
    {
        base.Awake();
        currentSpeed = speed;
    }

    public override void RoomUpdate() { }

    public override void RoomFixedUpdate()
    {
        rb.velocity = Vector2.zero;

        UpdateTarget();

        Move();
    }

    void UpdateTarget()
    {
        if(playerController == null)
        {
            FindPlayerController();
        }

        SetDirection();
    }

    void FindPlayerController()
    {
        var controller = PlayerManager.Instance.Active;
        if(controller is BodyPlayerController)
        {
            playerController = (BodyPlayerController)controller;
            return;
        }
        else
        {
            playerController = null;
        }
    }

    void SetDirection()
    {
        var resetDst = (startingPos - transform.localPosition).sqrMagnitude;

        if (playerController)
        {
            var chaseDist = (playerController.transform.localPosition - startingPos).sqrMagnitude;
            if(chaseDist > maxChaseDistance * maxChaseDistance)
            {
                if(resetDst > 0.01f)
                {
                    direction = (startingPos - transform.localPosition).normalized;
                }
                else
                {
                    direction = Vector2.zero;
                }
            }
            else
            {
                direction = (playerController.transform.localPosition - transform.localPosition).normalized;
            }
        }
        else
        {
            if (resetDst > 0.01f)
            {
                direction = (startingPos - transform.localPosition).normalized;
            }
            else
            {
                direction = Vector2.zero;
            }
        }
    }

    protected override void HitLayerReset() { }

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
