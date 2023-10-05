using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSection : MonoBehaviour
{
    [SerializeField]
    Sprite closedSprite, openSprite;

    SpriteRenderer spriteRenderer;
    BoxCollider2D boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetDoorSprite(bool closed)
    {
        if (closed)
        {
            spriteRenderer.sprite = closedSprite;
            boxCollider.enabled = true;
        }
        else
        {
            spriteRenderer.sprite = openSprite;
            boxCollider.enabled = false;
        }
    }
}
