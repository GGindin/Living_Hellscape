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
        PlayerManager.Instance.GhostInstance.transform.position += Vector3.up * 2f;
        (PlayerManager.Instance.GhostInstance as GhostPlayerController).HasLeftPlayer = true;
        TextBoxController.instance.OpenTextBox(introText);
        GameStateController.Instance.HasGottenIntro = true;
    }
}
