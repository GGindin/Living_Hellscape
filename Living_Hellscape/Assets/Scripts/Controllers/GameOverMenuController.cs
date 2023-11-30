using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameOverMenuController : MonoBehaviour
{
    public static GameOverMenuController Instance { get; private set; }

    [SerializeField]
    Button defaultSelectedButton;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (VignetteController.Instance.enabled) return;
        UserInput userInput = InputController.GetUserInput();

        if (userInput.mainAction == ButtonState.Down)
        {
            var button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.Invoke();
            }
        }
    }

    public void OpenGameOverMenu()
    {
        gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(defaultSelectedButton.gameObject);
    }

    public void ReloadGame()
    {
        GameController.Instance.ReloadPlaySession();
    }

    public void QuitGame()
    {
        GameController.Instance.EndPlaySession(false);
    }
}
