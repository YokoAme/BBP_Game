using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.SceneManagement;

public class CanvasScalerFixer
{
    [MenuItem("Tools/🛠 Fix All CanvasScalers in Scenes")]
    public static void FixAllCanvasScalers()
    {
        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");
        int fixedCount = 0;

        foreach (string guid in sceneGuids)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(guid);
            var scene = EditorSceneManager.OpenScene(scenePath);

            CanvasScaler[] scalers = Object.FindObjectsOfType<CanvasScaler>(true);
            foreach (var scaler in scalers)
            {
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 0.5f;
                fixedCount++;
            }

            EditorSceneManager.SaveScene(scene);
        }

        Debug.Log($"✅ CanvasScaler обновлены во всех сценах. Исправлено: {fixedCount} шт.");
    }
}
