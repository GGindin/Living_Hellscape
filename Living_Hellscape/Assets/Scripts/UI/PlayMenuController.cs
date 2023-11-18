using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class PlayMenuController : MenuController
{
    public static PlayMenuController Instance { get; private set; }

    [SerializeField]
    TextMeshProUGUI fileText;

    [SerializeField]
    Button playButton;

    [SerializeField]
    Button deleteButton;

    [SerializeField]
    Button backButton;

    int saveFile;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void OpenMenu(int saveFile)
    {
        gameObject.SetActive(true);
        this.saveFile = saveFile;
        EventSystem.current.SetSelectedGameObject(playButton.gameObject);
        currentSelected = playButton.gameObject;
        fileText.text = "Save " + (saveFile + 1);
    }

    public void CloseMenu()
    {
        saveFile = 0;
        gameObject.SetActive(false);
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

    public void PlaySave()
    {
        if (SceneController.Instance) SceneController.Instance.LoadPlaySessionScene(saveFile);
        else
        {
            Debug.Log("Need to start play session from scene controller scene for button to work");
        }
    }

    public void DeleteSave()
    {
        GameStorageController.Instance.DeleteSaveFile(saveFile);
        Back();
    }

    public void Back()
    {
        SaveMenuController.Instance.gameObject.SetActive(true);
        CloseMenu();
    }
}
