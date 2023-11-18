using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SaveMenuController : MenuController
{
    public static SaveMenuController Instance { get; private set; }

    [SerializeField]
    Button[] saveButtons;

    [SerializeField]
    Button backButton;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(saveButtons[0].gameObject);
        currentSelected = saveButtons[0].gameObject;
        SetupSaveButtons();
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
        else if(userInput.secondaryAction == ButtonState.Down)
        {
            var button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
            if (button != null)
            {
                for(int i = 0; i <  saveButtons.Length; i++)
                {
                    if (saveButtons[i] == button)
                    {
                        GameStorageController.Instance.DeleteSaveFile(i);
                        OnEnable();
                        break;
                    }
                }
            }
        }
    }

    public void PlaySave(int i)
    {
        if (SceneController.Instance)
        {
            if (GameStorageController.Instance.DoesSaveExist(i))
            {
                PlayMenuController.Instance.OpenMenu(i);
                gameObject.SetActive(false);
            }
            else
            {
                SceneController.Instance.LoadPlaySessionScene(i);
            }           
        }
        else
        {
            Debug.Log("Need to start play session from scene controller scene for button to work");
        }
    }

    public void Back()
    {
        MainMenuController.Instance.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    private void SetupSaveButtons()
    {
        if (GameStorageController.Instance == null) return;

        for(int i = 0; i < saveButtons.Length; i++)
        {
            var button = saveButtons[i];
            if (GameStorageController.Instance.DoesSaveExist(i))
            {
                button.GetComponentInChildren<Text>().text = "Save " + (i + 1);
            }
            else
            {
                button.GetComponentInChildren<Text>().text = "Create Save " + (i + 1);
            }
        }
    }
}
