using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptingController : MonoBehaviour
{
    public static ScriptingController Instance { get; private set; }

    [SerializeField]
    string introText;

    private void Awake()
    {
        Instance = this;
    }

    public void RunIntro()
    {
        GameController.Instance.GoToGhostNow();
        TextBoxController.instance.OpenTextBox(introText);
        GameStateController.Instance.HasGottenIntro = true;
    }
}