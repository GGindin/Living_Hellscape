using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingStation : InteractableObject
{
    [SerializeField]
    SpriteRenderer spriteRenderer;

    [SerializeField]
    Collider2D healCollider;

    [SerializeField]
    string healText;

    public override Collider2D InteractableCollider => healCollider;

    public override SpriteRenderer SpriteRenderer => spriteRenderer;

    public override void Interact()
    {
        TextBoxController.instance.OpenTextBoxWithCallBack(healText, () => HealPlayer());
    }

    void HealPlayer()
    {
        PlayerManager.Instance.BodyInstance.PlayerStats.ChangeCurrentHealth(PlayerManager.Instance.BodyInstance.PlayerStats.MaxHealth);
        PlayerManager.Instance.GhostInstance.PlayerStats.ChangeCurrentHealth(PlayerManager.Instance.GhostInstance.PlayerStats.MaxHealth);
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
