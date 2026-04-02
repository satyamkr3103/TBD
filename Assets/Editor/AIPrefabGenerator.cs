using UnityEngine;
using UnityEditor;
using System.IO;

public class AIPrefabGenerator
{
    private const string SPAWNABLES_FOLDER = "Assets/Resources/Spawnables";

    [MenuItem("Assets/Generate AI Prefab from Sprite", false, 20)]
    private static void GeneratePrefabFromSprite()
    {
        // Get all selected assets
        Object[] selectedObjects = Selection.objects;

        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("Please select at least one Sprite in the Project window.");
            return;
        }

        // Ensure the target folder exists
        if (!Directory.Exists(SPAWNABLES_FOLDER))
        {
            Directory.CreateDirectory(SPAWNABLES_FOLDER);
            AssetDatabase.Refresh();
        }

        int successCount = 0;

        foreach (Object obj in selectedObjects)
        {
            // We only care about textures/sprites
            if (obj is Texture2D || obj is Sprite)
            {
                string assetPath = AssetDatabase.GetAssetPath(obj);
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);

                if (sprite == null)
                {
                    Debug.LogWarning($"Skipped {obj.name}: Asset is not configured as a Sprite (2D and UI).");
                    continue;
                }

                // 1. Create temporary GameObject
                GameObject tempObj = new GameObject(sprite.name);

                // 2. Add required components
                SpriteRenderer sr = tempObj.AddComponent<SpriteRenderer>();
                sr.sprite = sprite;

                // Add physics based on approximate shape (Polygon is most accurate for random 2D art)
                tempObj.AddComponent<PolygonCollider2D>();
                
                // Add our custom logic script
                tempObj.AddComponent<DynamicObstacle>();

                // 3. Save as Prefab
                string prefabPath = $"{SPAWNABLES_FOLDER}/{sprite.name}.prefab";
                
                // If it already exists, this avoids creating duplicates
                prefabPath = AssetDatabase.GenerateUniqueAssetPath(prefabPath);
                
                PrefabUtility.SaveAsPrefabAsset(tempObj, prefabPath);
                
                // 4. Cleanup temporary object from scene
                Object.DestroyImmediate(tempObj);
                
                successCount++;
            }
        }

        if (successCount > 0)
        {
            Debug.Log($"<color=green>Successfully generated {successCount} AI Prefabs directly into {SPAWNABLES_FOLDER}!</color>");
            AssetDatabase.Refresh();
        }
    }

    [MenuItem("Assets/Generate AI Prefab from Sprite", true)]
    private static bool ValidateGeneratePrefabFromSprite()
    {
        // Only show this right-click option if a Texture/Sprite is selected
        return Selection.activeObject is Texture2D || Selection.activeObject is Sprite;
    }
}
