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
        mainAction.SetImage(sprite);
    }

    public void SetSecondAction(Sprite sprite)
    {
        secondAction.SetImage(sprite);
    }

    void CreateActionIcons()
    {
        mainAction = Instantiate(iconPrefab, transform);
        secondAction = Instantiate(iconPrefab, transform);

        mainAction.SetImage(null);
        secondAction.SetImage(null);
    }
}
