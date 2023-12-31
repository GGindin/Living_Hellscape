using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : InteractableObject
{
    [SerializeField, TextArea(1, 5)]
    protected string[] texts;

    protected int currentText;

    [SerializeField]
    BoxCollider2D boxCollider2D;

    [SerializeField]
    SpriteRenderer spriteRenderer;

    public override Collider2D InteractableCollider => boxCollider2D;

    public override SpriteRenderer SpriteRenderer => spriteRenderer;

    public override void Interact()
    {
        CheckForTextUpdate();
        TextBoxController.instance.OpenTextBoxWithCallBack(texts[currentText], () => OnFinishText());
    }

    public virtual void CheckForTextUpdate() { }

    public virtual void OnFinishText() { }

    public override string GetFileName()
    {
        throw new System.NotImplementedException();
    }

    public override void LoadPerm(GameDataReader reader)
    {
        currentText = reader.ReadInt();
    }

    public override void LoadTemp(GameDataReader reader)
    {
        throw new System.NotImplementedException();
    }

    public override void SavePerm(GameDataWriter writer)
    {
        writer.WriteInt(currentText);
    }

    public override void SaveTemp(GameDataWriter writer)
    {
        throw new System.NotImplementedException();
    }
}
