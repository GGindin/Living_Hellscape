using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuController : MenuController
{
    public static MainMenuController Instance { get; private set; }

    [SerializeField]
    Button defaultSelectedButton;

    private void Awake()
    {
        Instance = this;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        currentSelected = defaultSelectedButton.gameObject;
    }

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(defaultSelectedButton.gameObject);
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

    public void LoadSaveMenu()
    {
        if (!SceneController.Instance)
        {
            Debug.Log("Need to start play session from scene controller scene for button to work");
            return;
        }
        else
        {
            SaveMenuController.Instance.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    public void OpenControlsMenu()
    {
        ControlsMenuController.Instance.OpenMenu();
        gameObject.SetActive(false);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif

    }
}
