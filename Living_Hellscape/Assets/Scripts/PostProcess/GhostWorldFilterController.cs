using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class GhostWorldFilterController : MonoBehaviour
{
    public static GhostWorldFilterController Instance { get; private set; }

    [SerializeField]
    Material ghostFilterMaterial;

    [SerializeField]
    float transitionLength;

    int amountID = Shader.PropertyToID("_GhostAmount");

    Camera cam;

    bool setEnabledToFalse = false;

    public float TransitionLength => transitionLength;

    public float Amount => ghostFilterMaterial.GetFloat(amountID);

    private void Awake()
    {
        Instance = this;
        cam = GetComponent<Camera>();
        enabled = false;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, ghostFilterMaterial);

        if (setEnabledToFalse)
        {
            enabled = false;
            setEnabledToFalse = false;
        }
    }

    public void StartFilter()
    {
        enabled = true;
        GameController.Instance.SetStopUpdates(true);
        StartCoroutine(ProcessFilter());
    }

    public void SetFilterFull()
    {
        enabled = true;
        ghostFilterMaterial.SetFloat(amountID, 1f);
    }

    public void SetFilterNone()
    {
        enabled = false;
        setEnabledToFalse = false;
        ghostFilterMaterial.SetFloat(amountID, 0f);
    }

    public void EndFilter()
    {
        GameController.Instance.SetStopUpdates(true);
        StartCoroutine(UnProcessFilter());
    }

    IEnumerator ProcessFilter()
    {
        float duration = transitionLength;
        float current = 0f;

        while (current < duration)
        {
            current += Time.deltaTime;
            float t = Mathf.InverseLerp(0f, duration, current);
            ghostFilterMaterial.SetFloat(amountID, t);
            yield return null;
        }

        GameController.Instance.SetStopUpdates(false);
        ghostFilterMaterial.SetFloat(amountID, 1f);
    }

    IEnumerator UnProcessFilter()
    {
        float current = transitionLength;

        while (current > 0f)
        {
            current -= Time.deltaTime;
            float t = Mathf.InverseLerp(0f, transitionLength, current);
            ghostFilterMaterial.SetFloat(amountID, t);
            yield return null;
        }

        GameController.Instance.SetStopUpdates(false);
        ghostFilterMaterial.SetFloat(amountID, 0f);
        setEnabledToFalse = true;
    }
}
