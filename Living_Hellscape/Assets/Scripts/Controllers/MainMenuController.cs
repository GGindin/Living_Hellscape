using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField]
    Button defaultSelectedButton;

    private void Awake()
    {
        Cursor.visible = false;
    }

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(defaultSelectedButton.gameObject);
    }

    private void Update()
    {
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

    public void PlayGame()
    {
        if(SceneController.Instance) SceneController.Instance.LoadPlaySessionScene();
        else
        {
            Debug.Log("Need to start play session from scene controller scene for button to work");
        }
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
