using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Item : MonoBehaviour
{
    public Sprite uiIcon;

    [SerializeField]
    int id;

    [SerializeField]
    string description;

    [SerializeField]
    bool isMainAction;

    protected Animator animator;
    protected int presentID = Animator.StringToHash("present");

    int count;

    public bool IsMainAction => isMainAction;

    public int ID => id;

    public int Count => count;

    public string Description => description;   

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
    }

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

        if (PlayerManager.Instance.BodyInstance.Inventory.DoesInventoryContainItem(this))
        {
            transform.SetParent(PlayerManager.Instance.BodyInstance.transform, false);
        }
        else
        {
            transform.SetParent(PlayerManager.Instance.GhostInstance.transform, false);
        }
        
    }

    public virtual void Deactivate()
    {
        //we do not set this off because we need the animator to run
        //gameObject.SetActive(false);
        transform.SetParent(PlayerManager.Instance.transform, false);
    }

    public void StartPresent()
    {
        animator.SetBool(presentID, true);
    }

    public void StopPresent()
    {
        animator.SetBool(presentID, false);
    }

    public virtual void OnFirstAddToInventory() { }
}
