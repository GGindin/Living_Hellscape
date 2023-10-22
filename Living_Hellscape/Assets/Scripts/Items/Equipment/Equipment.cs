using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Equipment : Item
{
    protected Animator animator;

    [SerializeField]
    protected Damage damage;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public abstract override void SetDirection(Vector2 direction);

    public abstract override void TriggerAction();

    public abstract void EndAction();

    public override void Activate()
    {
        base.Activate();

        if (IsMainAction)
        {
            ActionPanelController.Instance.SetMainActionIcon(uiIcon);
        }
        else
        {
            ActionPanelController.Instance.SetSecondAction(uiIcon);
        }
    }

    public override void Deactivate()
    {
        base.Deactivate();

        if (ActionPanelController.Instance)
        {
            if (IsMainAction)
            {
                ActionPanelController.Instance.SetMainActionIcon(null);
            }
            else
            {
                ActionPanelController.Instance.SetSecondAction(null);
            }

        }
    }
}
