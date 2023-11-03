using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    public static PauseMenuController Instance { get; private set; }

    [SerializeField]
    Button defaultSelectedButton;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        UserInput userInput = InputController.GetUserInput();

        if(userInput.mainAction == ButtonState.Down)
        {
            var button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.Invoke();
            }
        }
    }

    public void OpenPauseMenu()
    {
        gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(defaultSelectedButton.gameObject);
    }

    public void ClosePauseMenu()
    {
        gameObject.SetActive(false);
    }

    public void ResumeGame()
    {
        GameController.Instance.SetPause(false);
        GameController.Instance.HandlePause();
    }

    public void QuitGame()
    {
        GameController.Instance.EndPlaySession();
    }
}
