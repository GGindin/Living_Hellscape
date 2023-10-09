using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostPlayerController : PlayerController
{
    bool hasLeftPlayer = false;

    public override void ActivateController()
    {
        gameObject.SetActive(true);
        transform.position = PlayerManager.instance.BodyPosition;
        isActive = true;
        hasLeftPlayer = false;
    }

    public override void DeactivateController()
    {
        gameObject.SetActive(false);
        isActive = false;
        hasLeftPlayer = false;
    }

    override protected void Awake()
    {
        base.Awake();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //TODO
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActive) return;

        if (hasLeftPlayer)
        {
            var bodyController = collision.gameObject.GetComponent<BodyPlayerController>();
            if (bodyController)
            {
                PlayerManager.instance.SetActiveController(bodyController);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isActive) return;

        var bodyController = collision.gameObject.GetComponent<BodyPlayerController>();
        if (bodyController)
        {
            hasLeftPlayer = true;
        }
    }
}
