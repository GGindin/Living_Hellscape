using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPanelController : MonoBehaviour
{
    public static ActionPanelController Instance { get; private set; }

    [SerializeField]
    ActionIcon iconPrefab;

    ActionIcon mainAction;

    ActionIcon secondAction;

    private void Awake()
    {
        Instance = this;
        CreateActionIcons();
    }

    public void SetMainActionIcon(Sprite sprite)
    {
        if(sprite == null)
        {
            Debug.Log("SETTING NULL SPRITE");
        }
        else
        {
            Debug.Log("SETTING REAL ICON");
        }
        mainAction.SetImage(sprite);
    }

    public void SetSecondAction(Sprite sprite)
    {
        secondAction.SetImage(sprite);
    }

    public void UpdateFromEquipedGear(EquipedGear equipedGear)
    {
        if (equipedGear.mainAction)
        {
            SetMainActionIcon(equipedGear.mainAction.uiIcon);
        }
        else
        {
            SetMainActionIcon(null);
        }

        if (equipedGear.secondAction)
        {
            SetSecondAction(equipedGear.secondAction.uiIcon);
        }
        else
        {
            SetSecondAction(null);
        }
    }

    void CreateActionIcons()
    {
        mainAction = Instantiate(iconPrefab, transform);
        secondAction = Instantiate(iconPrefab, transform);

        mainAction.SetImage(null);
        secondAction.SetImage(null);
    }
}
