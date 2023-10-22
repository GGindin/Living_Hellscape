using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : InteractableObject
{
    [SerializeField]
    Item chestItemPrefab;

    [SerializeField]
    BoxCollider2D boxCollider;

    [SerializeField]
    Animator animator;

    bool isClosed = true;

    int closedAnimID = Animator.StringToHash("closed");

    public override Collider2D InteractableCollider => boxCollider;

    public override void Interact()
    {
        if (!isClosed) return;

        Open();
    }

    void Open()
    {
        isClosed = false;
        SetAnimator();

        if (chestItemPrefab)
        {
            var item = Instantiate(chestItemPrefab);
            PlayerManager.Instance.Inventory.AddItem(item);
        }
    }

    void SetAnimator()
    {
        animator.SetBool(closedAnimID, isClosed);
    }
}
