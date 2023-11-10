using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class VignetteController : MonoBehaviour
{
    public static VignetteController Instance { get; private set; }

    [SerializeField]
    Material vignetteMaterial;

    [SerializeField]
    float duration = 2.0f;

    int focusID = Shader.PropertyToID("_FocusPos");
    int amountID = Shader.PropertyToID("_Amount");

    Camera cam;

    bool setEnabledToFalse = false;

    public float Duration => duration;

    private void Awake()
    {
        Instance = this;
        cam = GetComponent<Camera>();
        enabled = false;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Vector3 targetPos;

        if (!PlayerManager.Instance || !PlayerManager.Instance.Active)
        {
            targetPos = cam.WorldToViewportPoint(cam.transform.position + cam.transform.forward);
        }
        else
        {
            targetPos = cam.WorldToViewportPoint(PlayerManager.Instance.Active.transform.position);
        }



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
        enabled = true;
        StartCoroutine(UnProcessVignette());
    }

    public IEnumerator ProcessVignette()
    {
        enabled = true;
        float duration = this.duration;
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

    public IEnumerator UnProcessVignette()
    {
        enabled = true;
        float current = duration;

        while (current > 0f)
        {
            current -= Time.deltaTime;
            float t = Mathf.InverseLerp(0f, duration, current);
            vignetteMaterial.SetFloat(amountID, t);
            yield return null;
        }

        vignetteMaterial.SetFloat(amountID, 0f);
        setEnabledToFalse = true;
    }
}
