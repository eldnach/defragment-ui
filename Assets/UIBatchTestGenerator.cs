using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class UIBatchTestGenerator : MonoBehaviour
{
    [Header("Placement Boundary")]
    public RectTransform boundaryPanel;
    
    [Header("Grid Layout")]
    public int columns = 50;
    public int rows = 50;
    
    [Range(0f, 2f)]
    public float overlapFactor = 0.42f;
    
    [Header("Atlases (Must be on different Textures)")]
    public Sprite atlasA;
    public Sprite atlasB;
    public Sprite atlasC; // New third sprite

    public void GenerateOverlappingGrid()
    {
        if (boundaryPanel == null) boundaryPanel = GetComponent<RectTransform>();

        // 1. Clear existing
        for (int i = boundaryPanel.childCount - 1; i >= 0; i--)
        {
            if (Application.isPlaying) Destroy(boundaryPanel.GetChild(i).gameObject);
            else DestroyImmediate(boundaryPanel.GetChild(i).gameObject);
        }

        // 2. Setup sprite list for easier indexing
        List<Sprite> sprites = new List<Sprite>();
        if (atlasA != null) sprites.Add(atlasA);
        if (atlasB != null) sprites.Add(atlasB);
        if (atlasC != null) sprites.Add(atlasC);

        if (sprites.Count == 0) { Debug.LogError("Assign at least one sprite!"); return; }

        Rect containerRect = boundaryPanel.rect;
        float baseWidth = containerRect.width / columns;
        float baseHeight = containerRect.height / rows;
        Vector2 elementSize = new Vector2(baseWidth * (1 + overlapFactor), baseHeight * (1 + overlapFactor));

        Vector2 startPos = new Vector2(
            containerRect.xMin + (baseWidth / 2f), 
            containerRect.yMax - (baseHeight / 2f)
        );

        // 3. Spawn Loop
        int totalCount = columns * rows;
        for (int i = 0; i < totalCount; i++)
        {
            int col = i % columns;
            int row = i / columns;

            GameObject go = new GameObject($"UI_{i:000}_Rnd", typeof(RectTransform), typeof(Image));
            go.transform.SetParent(boundaryPanel, false);
            
            Image img = go.GetComponent<Image>();
            
            // Randomly pick from our list of sprites
            int randomIndex = Random.Range(0, sprites.Count);
            img.sprite = sprites[randomIndex];
            
            // Color them differently to make the chaos visible
            if (randomIndex == 0) img.color = Color.white;
            else if (randomIndex == 1) img.color = Color.red;
            else img.color = Color.blue;

            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = elementSize;
            
            float xOffset = col * baseWidth;
            float yOffset = row * baseHeight;
            rt.anchoredPosition = new Vector2(startPos.x + xOffset, startPos.y - yOffset);
        }
    }
}

[CustomEditor(typeof(UIBatchTestGenerator))]
public class UIBatchTestGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        UIBatchTestGenerator script = (UIBatchTestGenerator)target;
        
        EditorGUILayout.Space();
        GUI.backgroundColor = new Color(0.5f, 0.8f, 1f); // Blueish
        if (GUILayout.Button("Generate 3-Atlas Chaos")) script.GenerateOverlappingGrid();
    }
}