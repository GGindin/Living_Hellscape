using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public const int SCENE_CONTROLLER_SCENE_ID = 0;
    public const int PLAY_SEESION_SCENE_ID = 1;

    public static SceneController Instance { get; private set; }

    [SerializeField]
    Camera sceneControllerSceneCamera;

    int activeSceneID = -1;

    private void Awake()
    {
        Instance = this;
        StartCoroutine(LoadPlaySessionScene());
    }

    IEnumerator LoadPlaySessionScene()
    {
        var progress = SceneManager.LoadSceneAsync(PLAY_SEESION_SCENE_ID, LoadSceneMode.Additive);
        var scene = SceneManager.GetSceneByBuildIndex(PLAY_SEESION_SCENE_ID);

        while (!progress.isDone)
        {
            yield return null;
        }

        SceneManager.SetActiveScene(scene);

        activeSceneID = PLAY_SEESION_SCENE_ID;
        sceneControllerSceneCamera.gameObject.SetActive(false);
        GameController.Instance.StartPlaySession();
    }
}
