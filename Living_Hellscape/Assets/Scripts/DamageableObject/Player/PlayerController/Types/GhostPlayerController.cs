using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostPlayerController : PlayerController
{
    SpriteRenderer spriteRenderer;

    bool hasLeftPlayer = false;

    public bool HasLeftPlayer { get => hasLeftPlayer; set => hasLeftPlayer = value; }

    GhostFreeZone lastGhostFreeZone;

    override protected void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    }

    public override void ActivateController()
    {
        gameObject.SetActive(true);
        transform.position = PlayerManager.Instance.BodyPosition;
        hasLeftPlayer = false;
        if(lastGhostFreeZone != null)
        {
            if (!rb.IsTouching(lastGhostFreeZone.GetComponent<Collider2D>()))
            {
                EnemyGhostManager.Instance.PlayerInGhostFreeZone = false;
                lastGhostFreeZone.ZoneEnterTime = -1f;
                lastGhostFreeZone = null;
            }
            else
            {
                lastGhostFreeZone.ZoneEnterTime = Time.time;
            }
        }
    }

    public override void DeactivateController()
    {
        gameObject.SetActive(false);
        hasLeftPlayer = false;
    }

    protected override void SetIgnorePhysics()
    {
        Physics2D.IgnoreLayerCollision(11, 13);
        Physics2D.IgnoreLayerCollision(13, 22);
    }

    protected override void UnSetIgnorePhysics()
    {
        Physics2D.IgnoreLayerCollision(11, 13, false);
        Physics2D.IgnoreLayerCollision(13, 22, false);
    }

    public void FadeInImmediate()
    {
        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    public void FadeOutImmediate()
    {
        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    }

    public IEnumerator ProcessFadeIn()
    {
        float duration = GhostWorldFilterController.Instance.TransitionLength;
        float current = 0f;
        var startColor = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        var targetColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        while (current < duration)
        {
            current += Time.deltaTime;
            float t = Mathf.InverseLerp(0f, duration, current);
            spriteRenderer.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        spriteRenderer.color = targetColor;
    }

    public IEnumerator ProcessFadeOut()
    {
        float current = GhostWorldFilterController.Instance.TransitionLength;

        var startColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        var targetColor = new Color(1.0f, 1.0f, 1.0f, 0.0f);

        while (current > 0f)
        {
            current -= Time.deltaTime;
            float t = Mathf.InverseLerp(0f, 2f, current);
            spriteRenderer.color = Color.Lerp(targetColor, startColor, t);
            yield return null;
        }

        spriteRenderer.color = targetColor;
    }

    override protected void Move(Vector2 movement)
    {
        base.Move(movement);
        AudioController.Instance.StopWalkSound();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsActive) return;

        //probably need some check to stop the player from leaving a room whie holding something
        var interactableObject = collision.gameObject.GetComponent<InteractableObject>();
        if (interactableObject)
        {


            if (this.interactableObject == null)
            {
                NotificationBoxController.instance.CloseNotificationBox();
                this.interactableObject = interactableObject;
            }

            NotificationBoxController.instance.OpenNotificationBox("Press K to interact");

            if (!PlayerManager.Instance.PlayerHasControl)
            {
                NotificationBoxController.instance.CloseNotificationBox();
                this.interactableObject.Interact();
            }

            return;
        }

        var enemy = collision.gameObject.GetComponent<GhostEnemy>();
        if (enemy && !enemy.IsTakingDamage && !IsTakingDamage)
        {
            var contacts = collision.contacts;

            Vector2 damageDir = Vector2.zero;

            foreach (var contact in contacts)
            {
                damageDir += contact.normal;
            }

            var damage = enemy.Damage;
            AddStatusEffect(damage, damageDir.normalized);
        }
    }

    private void OnCollisionExit2D()
    {
        NotificationBoxController.instance.CloseNotificationBox();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!IsActive) return;

        var enemy = collision.gameObject.GetComponent<GhostEnemy>();
        if (enemy && !enemy.IsTakingDamage && !IsTakingDamage)
        {
            var contacts = collision.contacts;

            Vector2 damageDir = Vector2.zero;

            foreach (var contact in contacts)
            {
                damageDir += contact.normal;
            }

            var damage = enemy.Damage;

            AddStatusEffect(damage, damageDir.normalized);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsActive) return;

        var ghostFreeZone = collision.gameObject.GetComponent<GhostFreeZone>();
        if (ghostFreeZone)
        {
            lastGhostFreeZone = ghostFreeZone;
            lastGhostFreeZone.ZoneEnterTime = Time.time;
            EnemyGhostManager.Instance.PlayerInGhostFreeZone = true;
            return;
        }

        if (hasLeftPlayer && collision.attachedRigidbody && GameStateController.Instance.KnowsHowToPossesBody)
        {
            var bodyController = collision.attachedRigidbody.GetComponent<BodyPlayerController>();
            if (bodyController && playerStats.CurrentHealth > 0)
            {
                GameController.Instance.SwitchWorlds();
                //PlayerManager.Instance.SetActiveController(bodyController);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!IsActive) return;

        NotificationBoxController.instance.CloseNotificationBox();

        var ghostFreeZone = collision.gameObject.GetComponent<GhostFreeZone>();
        if (ghostFreeZone)
        {
            lastGhostFreeZone.ZoneEnterTime = -1f;
            lastGhostFreeZone = null; 
            EnemyGhostManager.Instance.PlayerInGhostFreeZone = false;
            return;
        }

        if (collision.attachedRigidbody)
        {
            var bodyController = collision.attachedRigidbody.GetComponent<BodyPlayerController>();
            if (bodyController)
            {
                hasLeftPlayer = true;
            }
        }

    }
}
