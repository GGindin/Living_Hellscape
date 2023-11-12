using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : InteractableObject
{
    [SerializeField]
    Item itemPrefab;

    [SerializeField]
    BoxCollider2D boxCollider;

    bool isPickedUp;

    public override Collider2D InteractableCollider => boxCollider;

    public override SpriteRenderer SpriteRenderer => null;

    public override void Interact()
    {
        PickUp();
    }

    void PickUp()
    {
        if (itemPrefab)
        {
            var item = Instantiate(itemPrefab);
            PlayerManager.Instance.Inventory.StartAddItem(item);

            if(!(item is EndGameItem))
            {
                isPickedUp = true;
            }

            TextBoxController.instance.OpenTextBox("You got a " + item.Description);

            gameObject.SetActive(false); //set inactive so that it can save its state when the room saves
        }
    }

    public override string GetFileName()
    {
        throw new System.NotImplementedException();
    }

    //will need to work with this and rooms because this should just go away
    //it could destroy itself I suppose, need to check the room saveing and loading code
    public override void LoadPerm(GameDataReader reader)
    {
        int value = reader.ReadInt();

        if (value == 1)
        {
            isPickedUp = true;
            gameObject.SetActive(false);
        }
        else
        {
            isPickedUp = false;        
        }
    }

    public override void SavePerm(GameDataWriter writer)
    {
        if (isPickedUp)
        {
            writer.WriteInt(1);
        }
        else
        {
            writer.WriteInt(0);
        }
    }

    public override void LoadTemp(GameDataReader reader)
    {
        throw new System.NotImplementedException();
    }



    public override void SaveTemp(GameDataWriter writer)
    {
        throw new System.NotImplementedException();
    }
}
