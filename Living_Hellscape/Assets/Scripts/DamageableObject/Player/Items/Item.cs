using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class Item : MonoBehaviour
{
    public Sprite uiIcon;

    [SerializeField]
    bool isMainAction;

    int count;

    public bool IsMainAction => isMainAction;

    public int Count => count;

    public void UseCount()
    {
        count--;
        Mathf.Max(count, 0);
    }

    public void AddCount(int amount)
    {
        count += amount;
    }

    public virtual void SetDirection(Vector2 direction) { }

    public virtual void TriggerAction() { }

    public void Activate()
    {
        gameObject.SetActive(true);
        transform.SetParent(PlayerManager.Instance.Active.transform, false);

        if (IsMainAction)
        {
            ActionPanelController.Instance.SetMainActionIcon(uiIcon);
        }
        else
        {
            ActionPanelController.Instance.SetSecondAction(uiIcon);
        }
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
        transform.SetParent(PlayerManager.Instance.transform, false);

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
