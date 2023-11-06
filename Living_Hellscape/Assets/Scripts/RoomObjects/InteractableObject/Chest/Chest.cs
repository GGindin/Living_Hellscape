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

    public override SpriteRenderer SpriteRenderer => null;

    public override void Interact()
    {
        if (!isClosed) return;

        Open();
    }

    public override string GetFileName()
    {
        //never should get called
        throw new System.NotImplementedException();
    }

    public override void SavePerm(GameDataWriter writer)
    {
        if (isClosed)
        {
            writer.WriteInt(0);
        }
        else
        {
            writer.WriteInt(1);
        }
    }

    public override void LoadPerm(GameDataReader reader)
    {
        int value = reader.ReadInt();

        if (value == 1)
        {
            isClosed = false;
            SetAnimator();
        }
        else
        {
            isClosed = true;
            SetAnimator();
        }
    }

    public override void SaveTemp(GameDataWriter writer)
    {
        //should never get called
        throw new System.NotImplementedException();
    }

    public override void LoadTemp(GameDataReader reader)
    {
        //should never get called
        throw new System.NotImplementedException();
    }

    void Open()
    {
        isClosed = false;
        SetAnimator();

        if (chestItemPrefab)
        {
            var item = Instantiate(chestItemPrefab);
            PlayerManager.Instance.Inventory.StartAddItem(item);

            // If the object has text attached to it, run the attached textbox creator. This will do nothing otherwise.
            TextboxCreator textbox = gameObject.GetComponent<TextboxCreator>();
            if (textbox)
            {
                textbox.StartTextEvent();
            }
        }
    }

    void SetAnimator()
    {
        animator.SetBool(closedAnimID, isClosed);
    }
}
