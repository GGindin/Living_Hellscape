using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public const int SCENE_CONTROLLER_SCENE_ID = 0;
    public const int PLAY_SEESION_SCENE_ID = 1;
    public const int MAIN_MENU_SCENE_ID = 2;

    public static SceneController Instance { get; private set; }

    [SerializeField]
    Camera sceneControllerSceneCamera;

    private void Awake()
    {
        Instance = this;
        LoadMainMenuScene();
    }

    public void LoadPlaySessionScene()
    {
        StartCoroutine(SwapToPlaySessionScene());
    }

    public void LoadMainMenuScene()
    {
        StartCoroutine(SwapToMainMenuScene());
    }

    IEnumerator SwapToPlaySessionScene()
    {
        sceneControllerSceneCamera.gameObject.SetActive(true);
        yield return StartCoroutine(UnLoadSceneByBuildIndex(MAIN_MENU_SCENE_ID));
        yield return StartCoroutine(LoadSceneByBuildIndex(PLAY_SEESION_SCENE_ID));
        sceneControllerSceneCamera.gameObject.SetActive(false);
        GameController.Instance.StartPlaySession();
    }

    IEnumerator SwapToMainMenuScene()
    {
        sceneControllerSceneCamera.gameObject.SetActive(true);
        yield return StartCoroutine(UnLoadSceneByBuildIndex(PLAY_SEESION_SCENE_ID));
        yield return StartCoroutine(LoadSceneByBuildIndex(MAIN_MENU_SCENE_ID));
        sceneControllerSceneCamera.gameObject.SetActive(false);
    }

    IEnumerator LoadSceneByBuildIndex(int buildIndex)
    {
        var progress = SceneManager.LoadSceneAsync(buildIndex, LoadSceneMode.Additive);
        var scene = SceneManager.GetSceneByBuildIndex(buildIndex);

        while (!progress.isDone)
        {
            yield return null;
        }

        SceneManager.SetActiveScene(scene);
    }

    IEnumerator UnLoadSceneByBuildIndex(int buildIndex)
    {
        if (SceneManager.GetSceneByBuildIndex(buildIndex).isLoaded)
        {
             var progress = SceneManager.UnloadSceneAsync(buildIndex);
            while (!progress.isDone)
            {
                yield return null;
            }
        }
    }
}
