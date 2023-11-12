using System;
using UnityEngine;

[System.Serializable]
public class PlayerInventory: ISaveableObject
{
    [SerializeField]
    EquipedGear equipedGear;

    [SerializeField]
    Item[] items = new Item[16];

    int marbleAmmo;

    public int MarbleAmmo => marbleAmmo;

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

    public void StartAddItem(Item item)
    {
        PlayerManager.Instance.StartCoroutine(PlayerManager.Instance.Active.PresentItem(item));
    }

    public void EndAddItem(Item item)
    {
        AddItem(item);
    }

    void AddItem(Item item)
    {
        if(item is EndGameItem)
        {
            item.OnFirstAddToInventory();
            return;
        }

        if(item is Upgrade)
        {
            var upgrade = (Upgrade)item;
            upgrade.AddUpgradeToStats(PlayerManager.Instance.BodyInstance.PlayerStats);
            upgrade.AddUpgradeToStats(PlayerManager.Instance.GhostInstance.PlayerStats);
            GameObject.Destroy(upgrade.gameObject);
            return;
        }

        int index = FindStack(item);

        if (index < 0)
        {
            int open = FindOpenSlot();
            if (open < 0) return;
            item.OnFirstAddToInventory();
            items[open] = item;
            if(item.Count == 0)
            {
                item.AddCount(1);
            }
            item.Deactivate();
        }
        else
        {
            if (item.Count == 0)
            {
                item.AddCount(1);
            }
            items[index].AddCount(item.Count);
            GameObject.Destroy(item.gameObject);
        }

        InventoryPanelController.Instance.UpdatePanel(this);
    }

    public Item GetItemAtIndex(int index)
    {
        return items[index];
    }

    public t GetItemByType<t>(Type type) where t : Item
    {
        for(int i = 0; i < items.Length; i++)
        {
            if (items[i])
            {
                if (items[i].GetType() == type)
                {
                    return items[i] as t;
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
        else if (item is Bandage)
        {
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

    public void UpdateEquipedGear()
    {
        ActionPanelController.Instance.UpdateFromEquipedGear(equipedGear);
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

    public void AddAmmo(int amount)
    {
        marbleAmmo += amount;
        AmmoPanelController.Instance.UpdateCount(marbleAmmo);
    }

    public void UseAmmo(int amount)
    {
        marbleAmmo = Mathf.Max(0, marbleAmmo - amount);
        AmmoPanelController.Instance.UpdateCount(marbleAmmo);
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

    int FindItemByID(int id)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null && items[i].ID == id)
            {
                return i;
            }
        }

        return -1;
    }

    public string GetFileName()
    {
        throw new NotImplementedException();
    }

    public void SavePerm(GameDataWriter writer)
    {
        //save inventory
        for(int i = 0; i < items.Length; i++)
        {
            var item = items[i];
            if(item == null)
            {
                writer.WriteInt(-1);
            }
            else
            {
                writer.WriteInt(item.ID);
                writer.WriteInt(item.Count);
            }
        }

        //then save equiped gear
        if(equipedGear.mainAction == null)
        {
            writer.WriteInt(-1);
        }
        else
        {
            writer.WriteInt(equipedGear.mainAction.ID);
        }

        //then save equiped gear
        if (equipedGear.secondAction == null)
        {
            writer.WriteInt(-1);
        }
        else
        {
            writer.WriteInt(equipedGear.secondAction.ID);
        }

        //write ammo
        writer.WriteInt(marbleAmmo);
    }

    public void LoadPerm(GameDataReader reader)
    {
        int val = 0;

        //load inventory
        for (int i = 0; i < items.Length; i++)
        {
            val = reader.ReadInt();

            if (val < 0) continue;
            else
            {
                var item = GameObject.Instantiate(Resources.Load<Item>("Items/item" + val));
                val = reader.ReadInt();
                item.AddCount(val);
                AddItem(item);
            }
        }

        //then load equiped gear
        //main
        val = reader.ReadInt();
        if (val >= 0)
        {
            int index = FindItemByID(val);
            if(index >= 0)
            {
                HandleMainActionItemSwap(items[index]);
            }
        }

        //second
        val = reader.ReadInt();
        if (val >= 0)
        {
            int index = FindItemByID(val);
            if (index >= 0)
            {
                HandleSecondActionItemSwap(items[index]);
            }
        }

        //load ammo if body
        if(PlayerManager.Instance.BodyInstance.Inventory == this)
        {
            marbleAmmo = reader.ReadInt();
            if (GameStateController.Instance.HasSlingShot)
            {
                AmmoPanelController.Instance.UpdateCount(marbleAmmo);
            }

        }

    }

    public void SaveTemp(GameDataWriter writer)
    {
        throw new NotImplementedException();
    }

    public void LoadTemp(GameDataReader reader)
    {
        throw new NotImplementedException();
    }
}
