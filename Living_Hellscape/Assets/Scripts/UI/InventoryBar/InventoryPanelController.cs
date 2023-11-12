using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryPanelController : MonoBehaviour
{
    public static InventoryPanelController Instance { get; private set; }

    [SerializeField]
    InventoryIcon inventoryIconPrefab;

    InventoryIcon[] inventoryIcons = new InventoryIcon[16];

    private void Awake()
    {
        Instance = this;
        CreateIcons();
        gameObject.SetActive(false);
    }

    public void OpenInventory()
    {
        gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(inventoryIcons[0].gameObject);
        var inventory = PlayerManager.Instance.Inventory;
        UpdatePanel(inventory);
    }

    public void CloseInventory()
    {
        TextBoxController.instance.CloseTextBox();
        gameObject.SetActive(false);
    }

    public void UpdatePanel(PlayerInventory inventory)
    {
        if (PlayerManager.Instance.Active.Inventory != inventory) return;

        for (int i = 0; i < inventory.Items.Length; i++)
        {
            var item = inventory.Items[i];
            var icon = inventoryIcons[i];
            if (item)
            {
                icon.SetIcon(item.uiIcon);
            }
            else
            {
                icon.SetIcon(null);
            }
        }
    }

    void CreateIcons()
    {
        for (int i = 0; i < inventoryIcons.Length; i++)
        {
            inventoryIcons[i] = Instantiate(inventoryIconPrefab, transform); 
        }
    }



    private void Update()
    {
        UserInput userInput = InputController.GetUserInput();

        int index = EventSystem.current.currentSelectedGameObject.transform.GetSiblingIndex();
        var item = PlayerManager.Instance.Inventory.GetItemAtIndex(index);

        if(userInput.inventory == ButtonState.Down)
        {
            GameController.Instance.SetPause(false);
            TextBoxController.instance.CloseTextBox();
            CloseInventory();
            return;
        }

        if (item != null)// && !TextBoxController.instance.gameObject.activeInHierarchy)
        {
            string itemText = item.Description + " Count: " + item.Count + '\n';
            itemText += "Equip or use with ";
            if (item.IsMainAction) itemText += "j";
            else itemText += "k";
            TextBoxController.instance.OpenTextBoxImmediate(itemText);
        }
        else if (item == null)
        {
            TextBoxController.instance.CloseTextBox();
        }

        if (userInput.mainAction == ButtonState.Down)
        {
            if (item)
            {
                PlayerManager.Instance.Inventory.HandleMainActionItemSwap(item);
            }
        }
        else if(userInput.secondaryAction == ButtonState.Down)
        {
            if (item)
            {
                PlayerManager.Instance.Inventory.HandleSecondActionItemSwap(item);
            }
        }     
    }
}
