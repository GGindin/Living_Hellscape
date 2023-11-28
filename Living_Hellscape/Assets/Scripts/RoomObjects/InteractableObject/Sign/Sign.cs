using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sign : InteractableObject
{
    [SerializeField]
    SpriteRenderer spriteRenderer;

    [SerializeField]
    Collider2D signCollider;

    [SerializeField]
    string signText;

    [SerializeField]
    bool mustDefeatEnemies;

    public override Collider2D InteractableCollider => signCollider;

    public override SpriteRenderer SpriteRenderer => spriteRenderer;

    public override void Interact()
    {
        if (mustDefeatEnemies && RoomController.Instance.ActiveRoom.HasActiveEnemies())
        {
            NotificationBoxController.instance.CloseNotificationBox();
            NotificationBoxController.instance.OpenNotificationBox("Can't investigate right now!");
        }
        else
        {
            NotificationBoxController.instance.CloseNotificationBox();
            TextBoxController.instance.OpenTextBox(signText);
        }
    }


    public override string GetFileName()
    {
        throw new System.NotImplementedException();
    }



    public override void LoadPerm(GameDataReader reader)
    {

    }

    public override void LoadTemp(GameDataReader reader)
    {

    }

    public override void SavePerm(GameDataWriter writer)
    {

    }

    public override void SaveTemp(GameDataWriter writer)
    {

    }
}
