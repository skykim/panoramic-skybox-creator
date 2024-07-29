using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.IO;

public class PanoramicCapture : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private int captureResolution = 4096;
    [SerializeField] private int antiAliasing = 8;

    private RenderTexture cubeMapRT;
    private RenderTexture equirectRT;

    private void Start()
    {
        SetupCamera();
        SetupRenderTextures();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Capture();
        }
    }

    private void SetupCamera()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
        targetCamera.farClipPlane = 1000f;
    }

    private void SetupRenderTextures()
    {
        cubeMapRT = new RenderTexture(captureResolution, captureResolution, 24, RenderTextureFormat.ARGB32)
        {
            dimension = TextureDimension.Cube,
            antiAliasing = antiAliasing,
            useMipMap = false,
            autoGenerateMips = false
        };

        equirectRT = new RenderTexture(captureResolution * 2, captureResolution, 24, RenderTextureFormat.ARGB32)
        {
            dimension = TextureDimension.Tex2D,
            antiAliasing = antiAliasing,
            useMipMap = false,
            autoGenerateMips = false
        };
    }

    private void Capture()
    {
        if (targetCamera == null)
        {
            Debug.LogError("Camera is not set!");
            return;
        }

        var prevClearFlags = targetCamera.clearFlags;
        var prevBackgroundColor = targetCamera.backgroundColor;

        targetCamera.clearFlags = CameraClearFlags.Skybox;

        var prevAmbientMode = RenderSettings.ambientMode;
        var prevAmbientIntensity = RenderSettings.ambientIntensity;

        RenderSettings.ambientMode = AmbientMode.Skybox;
        RenderSettings.ambientIntensity = 1.0f;

        targetCamera.RenderToCubemap(cubeMapRT, 63, Camera.MonoOrStereoscopicEye.Mono);
        cubeMapRT.ConvertToEquirect(equirectRT);

        targetCamera.clearFlags = prevClearFlags;
        targetCamera.backgroundColor = prevBackgroundColor;

        RenderSettings.ambientMode = prevAmbientMode;
        RenderSettings.ambientIntensity = prevAmbientIntensity;

        Save(equirectRT);
    }

    private void Save(RenderTexture rt)
    {
        Texture2D tex = null;
        try
        {
            tex = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false);
            
            RenderTexture.active = rt;
            tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            tex.Apply();
            RenderTexture.active = null;

            byte[] bytes = tex.EncodeToPNG();
            string fileName = "Panorama.png";
            string fullPath = Path.Combine(Application.dataPath, "SkyBox");
            
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }
            
            string filePath = Path.Combine(fullPath, fileName);
            File.WriteAllBytes(filePath, bytes);

            Debug.Log($"Saved panorama to: {filePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save panorama: {e.Message}");
        }
        finally
        {
            if (tex != null)
            {
                Destroy(tex);
            }
        }
    }

    private void OnDisable()
    {
        if (cubeMapRT != null)
        {
            cubeMapRT.Release();
            Destroy(cubeMapRT);
        }
        if (equirectRT != null)
        {
            equirectRT.Release();
            Destroy(equirectRT);
        }
    }
}