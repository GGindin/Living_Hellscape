using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class VignetteController : MonoBehaviour
{
    public static VignetteController Instance { get; private set; }

    [SerializeField]
    Material vignetteMaterial;

    int focusID = Shader.PropertyToID("_FocusPos");
    int amountID = Shader.PropertyToID("_Amount");

    Camera cam;

    bool setEnabledToFalse = false;

    private void Awake()
    {
        Instance = this;
        cam = GetComponent<Camera>();
        enabled = false;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!PlayerManager.Instance.Active) return;

        var targetPos = cam.WorldToViewportPoint(PlayerManager.Instance.Active.transform.position);


        vignetteMaterial.SetVector(focusID, targetPos);
        Graphics.Blit(source, destination, vignetteMaterial);

        if (setEnabledToFalse)
        {
            enabled = false;
            setEnabledToFalse = false;
        }
    }

    public void StartVignette()
    {
        enabled = true;
        StartCoroutine(ProcessVignette());
    }

    public void EndVignette()
    {
        StartCoroutine(UnProcessVignette());
    }

    IEnumerator ProcessVignette()
    {
        float duration = 2f;
        float current = 0f;

        while(current < duration)
        {
            current += Time.deltaTime;
            float t = Mathf.InverseLerp(0f, duration, current);
            vignetteMaterial.SetFloat(amountID, t);
            yield return null;
        }

        vignetteMaterial.SetFloat(amountID, 1f);
    }

    IEnumerator UnProcessVignette()
    {
        float current = 2f;

        while (current > 0f)
        {
            current -= Time.deltaTime;
            float t = Mathf.InverseLerp(0f, 2f, current);
            vignetteMaterial.SetFloat(amountID, t);
            yield return null;
        }

        vignetteMaterial.SetFloat(amountID, 0f);
        setEnabledToFalse = true;
    }
}
