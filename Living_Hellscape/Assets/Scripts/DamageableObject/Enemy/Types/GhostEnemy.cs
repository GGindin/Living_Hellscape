using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostEnemy : EnemyController
{
    float currentSqrDistToPlayer;

    bool hasDoneFirstFade = false;

    //use this for when player in body
    public float CurrentSqrDistToPlayer => currentSqrDistToPlayer;

    protected override void Awake()
    {
        base.Awake();
        currentSpeed = speed;
        StartCoroutine(ProcessFadeIn());
    }

    public override void RoomUpdate() { }

    public override void RoomFixedUpdate()
    {
        if (!hasDoneFirstFade) return;

        rb.velocity = Vector2.zero;

        UpdateDirection();

        Move();
    }

    public void UpdateDistanceWhenPlayerInBody()
    {
        var dist = Mathf.Sqrt(currentSqrDistToPlayer) + currentSpeed * Time.deltaTime;
        currentSqrDistToPlayer = dist * dist;
    }

    public void RepositionGhost()
    {
        var playerPos = PlayerManager.Instance.Active.transform.position;
        var previousVector = -direction * Mathf.Sqrt(currentSqrDistToPlayer);
        transform.position = new Vector2(playerPos.x, playerPos.y) + previousVector;
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

        if (!hasDoneFirstFade)
        {
            hasDoneFirstFade = true;
        }
    }

    public IEnumerator ProcessFadeOut()
    {
        float current = GhostWorldFilterController.Instance.TransitionLength;

        var startColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        var targetColor = new Color(1.0f, 1.0f, 1.0f, 0.0f);

        while (current > 0f)
        {
            current -= Time.deltaTime;
            float t = Mathf.InverseLerp(0f, GhostWorldFilterController.Instance.TransitionLength, current);
            spriteRenderer.color = Color.Lerp(targetColor, startColor, t);
            yield return null;
        }

        spriteRenderer.color = targetColor;

    }

    public IEnumerator SelfDestroy()
    {
        yield return StartCoroutine(ProcessFadeOut());
        Destroy(gameObject);
    }

    void UpdateDirection()
    {
        direction = (PlayerManager.Instance.GhostInstance.transform.position - transform.position);
        currentSqrDistToPlayer = direction.sqrMagnitude;
        direction.Normalize();

        if (EnemyGhostManager.Instance.PlayerInGhostFreeZone)
        {
            direction = -direction;
        }
    }

    protected override void SetAnimatorDirection(Vector3 direction)
    {
        animator.SetFloat(xDirAnimID, direction.x);
        animator.SetFloat(yDirAnimID, direction.y);
    }

    protected override void HitLayerReset(Collision2D collision) { }

    public override void SavePerm(GameDataWriter writer) { }

    public override void LoadPerm(GameDataReader reader) { }

    public override void SaveTemp(GameDataWriter writer) { }

    public override void LoadTemp(GameDataReader reader) { }
}
