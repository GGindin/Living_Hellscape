using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class VignetteController : MonoBehaviour
{
    [SerializeField]
    Material vignetteMaterial;

    int focusID = Shader.PropertyToID("_FocusPos");

    Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!PlayerManager.Instance.Active) return;

        var targetPos = cam.WorldToViewportPoint(PlayerManager.Instance.Active.transform.position);


        vignetteMaterial.SetVector(focusID, targetPos);
        Graphics.Blit(source, destination, vignetteMaterial);
    }
}
