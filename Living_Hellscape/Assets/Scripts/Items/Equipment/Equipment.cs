using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Equipment : Item
{
    public abstract override void SetDirection(Vector2 direction);

    public abstract override void TriggerAction();

    public abstract void EndAction();

    public override void Activate()
    {
        base.Activate();
        /*
        if (IsMainAction)
        {
            ActionPanelController.Instance.SetMainActionIcon(uiIcon);
        }
        else
        {
            ActionPanelController.Instance.SetSecondAction(uiIcon);
        }
        */
    }

    public override void Deactivate()
    {
        base.Deactivate();

        /*
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
        */
    }

    public void SetActionIcon()
    {
        if (ActionPanelController.Instance)
        {
            if (IsMainAction)
            {

                ActionPanelController.Instance.SetMainActionIcon(uiIcon);
            }
            else
            {
                ActionPanelController.Instance.SetSecondAction(uiIcon);
            }
        }
    }

    public void TurnOffActionIcon()
    {
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
