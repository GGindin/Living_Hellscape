using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : BodyEquipment, IStatuser
{
    [SerializeField]
    Damage damage;

    public Damage Damage => new Damage(damage);

    int xDirID = Animator.StringToHash("xDir");
    int yDirID = Animator.StringToHash("yDir");
    int strikeID = Animator.StringToHash("strike");

    bool isActing = false;

    public override void SetDirection(Vector2 direction)
    {
        if (isActing) return;

        animator.SetFloat(xDirID, direction.x);
        animator.SetFloat(yDirID, direction.y);
    }

    public override void TriggerAction()
    {
        isActing = true;
        animator.SetTrigger(strikeID);
        AudioController.Instance.PlaySoundEffect("knifeattack");
        if (gameObject.activeInHierarchy)
        {
            PlayerManager.Instance.Active.StartCoroutine(PlayerManager.Instance.Active.StopControlForTime(.25f));
        }
    }

    public override void EndAction()
    {
        isActing = false;
    }

    public StatusEffect GetStatus(DamageableObject recievingObject)
    {
        return Damage;
    }

    public override void OnFirstAddToInventory()
    {
        if (!GameStateController.Instance.HasGotKnife)
        {
            GameStateController.Instance.HasGotKnife = true;
            TextBoxController.instance.OpenTextBox("Equip items by pressing + to open your inventory. Use Esc to pause and quit the game.");
        }
    }
}
