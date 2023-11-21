using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControlsMenuController : MenuController
{
    public static ControlsMenuController Instance { get; private set; }

    [SerializeField]
    Button backButton;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void OpenMenu()
    {
        gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(backButton.gameObject);
        currentSelected = backButton.gameObject;
    }

    public void CloseMenu()
    {
        gameObject.SetActive(false);
    }

    public void Back()
    {
        MainMenuController.Instance.gameObject.SetActive(true);
        CloseMenu();
    }

    private void Update()
    {
        if (VignetteController.Instance.isActiveAndEnabled) return;

        UserInput userInput = InputController.GetUserInput();
        CheckSelected();

        if (userInput.mainAction == ButtonState.Down)
        {
            var button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
            if (button != null)
            {
                PlayButtonDownSound();
                button.onClick.Invoke();
            }
        }
    }
}
