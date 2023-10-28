using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemDrop : MonoBehaviour
{
    public float dropDistance;

    public float dropTime;

    public IEnumerator Drop(Vector2 direction)
    {
        var start = new Vector2(transform.position.x, transform.position.y);
        var target = direction * dropDistance + start;

        float currentTime = 0f;
        while(currentTime < dropTime)
        {
            currentTime += Time.deltaTime;
            var t = Mathf.InverseLerp(0f, dropTime, currentTime);
            transform.position = Vector2.Lerp(start, target, t);

            yield return null;
        }

        transform.position = target;
    }

    public abstract void Collect();
}
