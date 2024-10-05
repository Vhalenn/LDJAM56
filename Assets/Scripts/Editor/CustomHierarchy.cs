
#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class CustomHierarchy : MonoBehaviour
{
    private static Vector2 offset = new Vector2(20, 1);

    static CustomHierarchy()
    {
        EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI;
    }

    private static void HandleHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {

        GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (obj != null)
        {
            Color backgroundColor = Color.white;
            Color textColor = Color.white;
            bool modified = false;

            if (!obj.activeSelf)
            {
                backgroundColor *= 0.5f;
                backgroundColor.a = 1f;
                textColor *= 0.9f;
            }
            Texture2D texture = null;

            if (obj.name == "World")
            {
                backgroundColor *= new Color(0.5f, 0.1f, 0.1f);
                textColor = new Color(0.9f, 0.9f, 0.9f);
                modified = true;
            }
            else if (obj.name.EndsWith("Render"))
            {
                backgroundColor *= new Color(0.2f, 0.3f, 0.7f);
                textColor = new Color(0.9f, 0.9f, 0.9f);
                modified = true;
            }
            else if (obj.name.StartsWith("PJ"))
            {
                backgroundColor *= new Color(0.2f, 0.6f, 0.1f);
                textColor *= new Color(0.9f, 0.9f, 0.9f);
                modified = true;
            }
            else if (obj.name.Contains("Canvas"))
            {
                backgroundColor *= new Color(0.7f, 0.45f, 0.0f);
                textColor *= new Color(0.9f, 0.9f, 0.9f);
                modified = true;
            }

            if (modified)
            {
                Rect offsetRect = new Rect(selectionRect.position + offset, selectionRect.size);
                Rect bgRect = new Rect(selectionRect.x, selectionRect.y, selectionRect.width + 50, selectionRect.height);

                EditorGUI.DrawRect(bgRect, backgroundColor);
                EditorGUI.LabelField(offsetRect, obj.name, new GUIStyle()
                {
                    normal = new GUIStyleState() { textColor = textColor },
                    fontStyle = FontStyle.Bold
                }
                );

                if (texture != null)
                    EditorGUI.DrawPreviewTexture(new Rect(selectionRect.position, new Vector2(selectionRect.height, selectionRect.height)), texture);
            }
        }
    }
}
#endif