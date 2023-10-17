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
    }

    public Item GetItemAtIndex(int index)
    {
        return items[index];
    }

    public void HandleMainActionItemSwap(Item item)
    {
        if (!item.IsMainAction) return;

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

    public void HandleSecondActionItemSwap(Item item)
    {
        if (item.IsMainAction) return;

        if (equipedGear.secondAction)
        {
            equipedGear.secondAction.Deactivate();
            var secondAction = equipedGear.secondAction;
            equipedGear.secondAction = null;
            if (secondAction == item) return;
        }

        equipedGear.secondAction = item;
        item.Activate();
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
            if(itemSlot.GetType() == typeof(Item))
            {
                return i;
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
