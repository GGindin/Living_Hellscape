using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeDoor : InteractableObject
{
    CompositeCollider2D compCollider;

    public override Collider2D InteractableCollider => compCollider;

    public override SpriteRenderer SpriteRenderer => null;

    protected virtual void Awake()
    {
        compCollider = GetComponent<CompositeCollider2D>();
    }


    public override void Interact()
    {
        TextBoxController.instance.OpenTextBox("This door won't budge. You feel you have business here you must attend to before you leave.");
    }


    public override string GetFileName()
    {
        throw new System.NotImplementedException();
    }



    public override void LoadPerm(GameDataReader reader)
    {
        throw new System.NotImplementedException();
    }

    public override void LoadTemp(GameDataReader reader)
    {
        throw new System.NotImplementedException();
    }

    public override void SavePerm(GameDataWriter writer)
    {
        throw new System.NotImplementedException();
    }

    public override void SaveTemp(GameDataWriter writer)
    {
        throw new System.NotImplementedException();
    }
}
