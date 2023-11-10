using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostPlayerController : PlayerController
{
    SpriteRenderer spriteRenderer;

    bool hasLeftPlayer = false;

    GhostFreeZone lastGhostFreeZone;

    override protected void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
                Debug.Log("ACTIVATE LEFT ZONE");
                EnemyGhostManager.Instance.PlayerInGhostFreeZone = false;
                lastGhostFreeZone = null;
            }
        }
    }

    public override void DeactivateController()
    {
        gameObject.SetActive(false);
        hasLeftPlayer = false;
    }

    public void FadeInImmediate()
    {
        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsActive) return;

        //probably need some check to stop the player from leaving a room whie holding something
        var interactableObject = collision.gameObject.GetComponent<InteractableObject>();
        if (interactableObject)
        {
            if (this.interactableObject == null)
            {
                this.interactableObject = interactableObject;
            }

            if (!PlayerManager.Instance.PlayerHasControl)
            {
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
            Debug.Log("ZONE ENTER");
            lastGhostFreeZone = ghostFreeZone;
            EnemyGhostManager.Instance.PlayerInGhostFreeZone = true;
        }

        if (hasLeftPlayer)
        {
            var bodyController = collision.gameObject.GetComponent<BodyPlayerController>();
            if (bodyController)
            {
                GameController.Instance.SwitchWorlds();
                //PlayerManager.Instance.SetActiveController(bodyController);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!IsActive) return;

        var ghostFreeZone = collision.gameObject.GetComponent<GhostFreeZone>();
        if (ghostFreeZone)
        {
            Debug.Log("ZONE LEAVE");
            lastGhostFreeZone = null;
            EnemyGhostManager.Instance.PlayerInGhostFreeZone = false;
        }

        var bodyController = collision.gameObject.GetComponent<BodyPlayerController>();
        if (bodyController)
        {
            hasLeftPlayer = true;
        }
    }
}
