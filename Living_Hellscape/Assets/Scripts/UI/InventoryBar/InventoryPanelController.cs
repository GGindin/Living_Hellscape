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
        gameObject.SetActive(false);
    }

    public void UpdatePanel(PlayerInventory inventory)
    {
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

        if(userInput.mainAction == ButtonState.Down)
        {
            int index = EventSystem.current.currentSelectedGameObject.transform.GetSiblingIndex();
            var item = PlayerManager.Instance.Inventory.GetItemAtIndex(index);
            PlayerManager.Instance.Inventory.HandleMainActionItemSwap(item);
        }
        else if(userInput.secondaryAction == ButtonState.Down)
        {
            int index = EventSystem.current.currentSelectedGameObject.transform.GetSiblingIndex();
            var item = PlayerManager.Instance.Inventory.GetItemAtIndex(index);
            PlayerManager.Instance.Inventory.HandleSecondActionItemSwap(item);
        }     
    }
}
