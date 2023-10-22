using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerInventory
{
    [SerializeField]
    EquipedGear equipedGear;

    [SerializeField]
    Item[] items = new Item[16];

    public Item[] Items => items;

    public bool HasMainAction => equipedGear.mainAction;

    public bool HasSecondAction => equipedGear.secondAction;

    //temporary
    public void InstantiateInventory()
    {
        for(int i = 0; i < items.Length; i++)
        {
            if (items[i] != null)
            {
                items[i] = GameObject.Instantiate(items[i]);
                items[i].AddCount(1);
                items[i].Deactivate();
            }
        }
    }

    public void AddItem(Item item)
    {
        int index = FindStack(item);

        if (index < 0)
        {
            int open = FindOpenSlot();
            if (open < 0) return;
            items[open] = item;
            item.Deactivate();
        }
        else
        {
            items[index].AddCount(item.Count);
            GameObject.Destroy(item.gameObject);
        }

        InventoryPanelController.Instance.UpdatePanel(this);
    }

    public Item GetItemAtIndex(int index)
    {
        return items[index];
    }

    public Item GetItemByType(Type type)
    {
        for(int i = 0; i < items.Length; i++)
        {
            if (items[i])
            {
                if (items[i].GetType() == type)
                {
                    return items[i];
                }
            }
        }

        return null;
    }

    public void HandleMainActionItemSwap(Item item)
    {
        if (!item.IsMainAction) return;

        if (item is Equipment)
        {
            if (equipedGear.mainAction)
            {
                equipedGear.mainAction.Deactivate();
                var mainAction = equipedGear.mainAction;
                equipedGear.mainAction = null;
                if (mainAction == item) return;
            }

            equipedGear.mainAction = item as Equipment;
            item.Activate();
        }
        else if(item is Consumable)
        {
            //use item if consumable
            item.Activate();

            if (item.Count <= 0)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    if (item = items[i])
                    {
                        items[i] = null;
                        GameObject.Destroy(item.gameObject);
                    }
                }
            }

            InventoryPanelController.Instance.UpdatePanel(this);
        }
    }

    public void HandleSecondActionItemSwap(Item item)
    {
        if (item.IsMainAction) return;

        if (item is Equipment)
        {
            if (equipedGear.secondAction)
            {
                equipedGear.secondAction.Deactivate();
                var secondAction = equipedGear.secondAction;
                equipedGear.secondAction = null;
                if (secondAction == item) return;
            }

            equipedGear.secondAction = item as Equipment;
            item.Activate();
        }
    }

    public void UpdateInventory()
    {
        for(int i = 0; i < items.Length; i++)
        {
            if (items[i] == null) continue;

            if (items[i].Count <= 0)
            {
                GameObject.Destroy(items[i].gameObject);
                items[i] = null;
            }
        }

        InventoryPanelController.Instance.UpdatePanel(this);
    }

    public void DoMainAction()
    {
        if (HasMainAction)
        {
            equipedGear.mainAction.TriggerAction();
        }
    }

    public void DoSecondAction()
    {
        if(HasSecondAction)
        {
            equipedGear.secondAction.TriggerAction();
        }
    }

    public void RotateEquipment(Vector2 direction)
    {
        if (HasMainAction)
        {
            equipedGear.mainAction.SetDirection(direction);
        }
        if (HasSecondAction)
        {
            equipedGear.secondAction.SetDirection(direction);
        }
    }

    int FindStack(Item item)
    {
        for (int i = 0; i < items.Length; i++)
        {
            var itemSlot = items[i];

            if (itemSlot)
            {
                if (itemSlot.GetType() == item.GetType())
                {
                    return i;
                }
            }
        }

        return -1;
    }

    int FindOpenSlot()
    {
        for(int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                return i;
            }
        }

        return -1;
    }
}
