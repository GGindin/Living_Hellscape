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

    private void Update()
    {
        SetAnimator();
    }

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
            writer.WriteInt(1);
        }
        else
        {
            writer.WriteInt(0);
        }
    }

    public override void LoadPerm(GameDataReader reader)
    {
        int value = reader.ReadInt();

        if (value == 1)
        {
            isClosed = true;
            SetAnimator();
        }
        else
        {
            isClosed = false;
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

        AudioController.Instance.PlaySoundEffect("victory");

        if (chestItemPrefab)
        {
            var item = Instantiate(chestItemPrefab);

            // If the object has text attached to it, run the attached textbox creator. This will do nothing otherwise.
            TextBoxController.instance.OpenTextBox("You picked up a " + chestItemPrefab.Description);

            PlayerManager.Instance.Inventory.StartAddItem(item);
        }
    }

    void SetAnimator()
    {
        animator.SetBool(closedAnimID, isClosed);
    }
}
