using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public virtual void Activate()
    {
        gameObject.SetActive(true);
        transform.SetParent(PlayerManager.Instance.Active.transform, false);
    }

    public virtual void Deactivate()
    {
        gameObject.SetActive(false);
        transform.SetParent(PlayerManager.Instance.transform, false);
    }
}
