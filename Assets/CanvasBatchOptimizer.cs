using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CanvasBatchOptimizer : MonoBehaviour
{
    [Tooltip("The parent object whose children we want to defragment (usually a Panel or Canvas).")]
    public RectTransform targetParent;

    public void OptimizeHierarchy()
    {
        if (targetParent == null) targetParent = GetComponent<RectTransform>();

        // 1. Collect all direct children
        List<RectTransform> children = new List<RectTransform>();
        foreach (RectTransform child in targetParent)
        {
            children.Add(child);
        }

        // 2. Group by "Material Key" (Texture + Shader)
        // This effectively creates the 'Buckets' we discussed
        var groupedChildren = children.OrderBy(child => GetMaterialKey(child)).ToList();

        // 3. Apply the new order
        // We set the Sibling Index to force the 'Tie' in the C++ Priority logic
        for (int i = 0; i < groupedChildren.Count; i++)
        {
            // Optional: Ensure Z is zero to bypass Distance Exit Ramps
            groupedChildren[i].localPosition = new Vector3(
                groupedChildren[i].localPosition.x, 
                groupedChildren[i].localPosition.y, 
                0
            );

            groupedChildren[i].SetSiblingIndex(i);
        }

        Debug.Log($"Optimized {children.Count} elements under {targetParent.name}");
    }

    private string GetMaterialKey(RectTransform rt)
    {
        // Try to find a Graphic component (Image, RawImage, Text)
        Graphic g = rt.GetComponent<Graphic>();
        if (g == null) return "None";

        // The 'Key' is the Texture ID + Shader ID
        Texture tex = g.mainTexture;
        Shader sh = g.material.shader;

        return $"{tex?.GetInstanceID() ?? 0}_{sh.GetInstanceID()}";
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(CanvasBatchOptimizer))]
public class CanvasBatchOptimizerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        CanvasBatchOptimizer script = (CanvasBatchOptimizer)target;

        if (GUILayout.Button("Optimize Hierarchy (Defragment Atlas)"))
        {
            Undo.RegisterFullObjectHierarchyUndo(script.targetParent, "Optimize UI Batching");
            script.OptimizeHierarchy();
        }
    }
}
#endif