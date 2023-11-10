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
    }

    private void Start()
    {
        LoadMainMenuScene();
    }

    public void LoadPlaySessionScene(int i)
    {
        StartCoroutine(SwapToPlaySessionScene(i));
    }

    public void LoadMainMenuScene()
    {
        StartCoroutine(SwapToMainMenuScene());
    }

    public void ReloadPlayerSessionScene()
    {
        StartCoroutine(SwapBackToPlaySessionScene(GameStorageController.Instance.saveFileInt));
    }

    IEnumerator SwapBackToPlaySessionScene(int i)
    {
        //in here we can change volumes of sounds and stuff
        if (SceneManager.GetSceneByBuildIndex(PLAY_SEESION_SCENE_ID).isLoaded)
        {
            VignetteController.Instance.StartVignette();
            yield return StartCoroutine(AudioController.Instance.FadeOutSoundEffect("MainMusic", VignetteController.Instance.Duration));
            sceneControllerSceneCamera.gameObject.SetActive(true);
        }

        yield return StartCoroutine(UnLoadSceneByBuildIndex(PLAY_SEESION_SCENE_ID));

        yield return null;

        yield return StartCoroutine(LoadSceneByBuildIndex(PLAY_SEESION_SCENE_ID));

        yield return null;

        //here move volumes and stuff back to where they were
        sceneControllerSceneCamera.gameObject.SetActive(false);
        GameController.Instance.StartPlaySession(i);
        GameController.Instance.SetStopUpdates(true);
        VignetteController.Instance.EndVignette();
        yield return StartCoroutine(AudioController.Instance.FadeInSoundEffect("MainMusic", VignetteController.Instance.Duration));
        GameController.Instance.SetStopUpdates(false);
    }

    IEnumerator SwapToPlaySessionScene(int i)
    {
        //in here we can change volumes of sounds and stuff
        if (SceneManager.GetSceneByBuildIndex(MAIN_MENU_SCENE_ID).isLoaded)
        {
            VignetteController.Instance.StartVignette();
            yield return StartCoroutine(AudioController.Instance.FadeOutSoundEffect("MainMusic", VignetteController.Instance.Duration));
            sceneControllerSceneCamera.gameObject.SetActive(true);
        }

        yield return StartCoroutine(UnLoadSceneByBuildIndex(MAIN_MENU_SCENE_ID));

        yield return null;

        yield return StartCoroutine(LoadSceneByBuildIndex(PLAY_SEESION_SCENE_ID));

        yield return null;

        //here move volumes and stuff back to where they were
        sceneControllerSceneCamera.gameObject.SetActive(false);
        GameController.Instance.StartPlaySession(i);
        GameController.Instance.SetStopUpdates(true);
        VignetteController.Instance.EndVignette();
        yield return StartCoroutine(AudioController.Instance.FadeInSoundEffect("MainMusic", VignetteController.Instance.Duration));
        GameController.Instance.SetStopUpdates(false);
    }

    IEnumerator SwapToMainMenuScene()
    {
        //in here we can change volumes of sounds and stuff
        if (SceneManager.GetSceneByBuildIndex(PLAY_SEESION_SCENE_ID).isLoaded)
        {
            VignetteController.Instance.StartVignette();
            yield return StartCoroutine(AudioController.Instance.FadeOutSoundEffect("MainMusic", VignetteController.Instance.Duration));
            sceneControllerSceneCamera.gameObject.SetActive(true);
        }

        yield return StartCoroutine(UnLoadSceneByBuildIndex(PLAY_SEESION_SCENE_ID));

        yield return null;

        yield return StartCoroutine(LoadSceneByBuildIndex(MAIN_MENU_SCENE_ID));

        yield return null;

        //here move volumes and stuff back to where they were
        sceneControllerSceneCamera.gameObject.SetActive(false);
        VignetteController.Instance.EndVignette();
        yield return StartCoroutine(AudioController.Instance.FadeInSoundEffect("MainMusic", VignetteController.Instance.Duration));
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
