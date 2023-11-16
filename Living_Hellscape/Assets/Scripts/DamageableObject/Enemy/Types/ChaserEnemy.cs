using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaserEnemy : EnemyController
{
    PlayerController playerController;

    [SerializeField]
    float maxChaseDistance;

    GameObject collisionGameObject;
    Vector2 colNormal;

    protected override void Awake()
    {
        base.Awake();
        currentSpeed = speed;
        collisionGameObject = null;
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
        FindPlayerController();

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
        var resetDst = (startingPos - transform.position).sqrMagnitude;

        if (playerController)
        {
            var chaseDist = (playerController.transform.position - startingPos).sqrMagnitude;

            if (chaseDist > maxChaseDistance * maxChaseDistance)
            {
                if(resetDst > 0.01f)
                {
                    direction = (startingPos - transform.position).normalized;
                }
                else
                {
                    direction = Vector2.zero;
                }
            }
            else
            {
                direction = (playerController.transform.position - transform.position).normalized;
                if (collisionGameObject)
                {
                    var dot = Vector2.Dot(colNormal, direction);
                    direction = direction - (colNormal * dot);
                    direction.Normalize();
                }
            }
        }
        else
        {
            if (resetDst > 0.01f)
            {
                direction = (startingPos - transform.position).normalized;
            }
            else
            {
                direction = Vector2.zero;
            }
        }
    }

    protected override void HitLayerReset(Collision2D collision)
    {
        var contacts = collision.contacts;
        var normal = new Vector2();

        foreach(var c in contacts)
        {
            normal += c.normal;
        }

        normal.Normalize();

        collisionGameObject = collision.gameObject;
        colNormal = normal;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collisionGameObject == collision.gameObject)
        {
            collisionGameObject = null;
            colNormal = new Vector2();
        }
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
