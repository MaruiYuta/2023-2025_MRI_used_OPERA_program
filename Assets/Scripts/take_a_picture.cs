using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class take_a_picture : MonoBehaviour
{
    private static string folderPath = "Screenshots";
    private static string[] cameraNames = { "Camera_back", "Camera_right", "Camera_front", "Camera_left" };

    // 任意の解像度を指定
    private static int resolutionWidth = 1920;  // 横解像度
    private static int resolutionHeight = 1080; // 縦解像度

#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Initialize()
    {
        Debug.Log("Script initialized: Listening for PlayMode changes.");
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            Debug.Log("Play mode exiting. Capturing screenshots.");
            CaptureScreenshotsFromSpecificCameras();
        }
    }

    private static void CaptureScreenshotsFromSpecificCameras()
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Debug.Log("Created folder: " + folderPath);
        }

        foreach (string cameraName in cameraNames)
        {
            Camera camera = GameObject.Find(cameraName)?.GetComponent<Camera>();
            if (camera != null)
            {
                CaptureScreenshotFromCamera(camera);
            }
            else
            {
                Debug.LogWarning("Camera with name " + cameraName + " not found!");
            }
        }
    }

    private static void CaptureScreenshotFromCamera(Camera camera)
    {
        // RenderTextureを利用して解像度を指定
        RenderTexture renderTexture = new RenderTexture(resolutionWidth, resolutionHeight, 24);
        camera.targetTexture = renderTexture;

        // カメラのレンダリング結果をTexture2Dに保存
        Texture2D screenshot = new Texture2D(resolutionWidth, resolutionHeight, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;

        camera.Render(); // カメラの描画を実行
        screenshot.ReadPixels(new Rect(0, 0, resolutionWidth, resolutionHeight), 0, 0);
        screenshot.Apply();

        // 保存するファイル名とパス
        string filePath = Path.Combine(folderPath, "Screenshot_" + camera.name + "_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png");
        File.WriteAllBytes(filePath, screenshot.EncodeToPNG());
        Debug.Log("Screenshot saved: " + filePath);

        // 後処理
        camera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);
        Destroy(screenshot);
    }
#endif
}
