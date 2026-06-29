using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RecipeData))]
public class RecipeDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RecipeData data = (RecipeData)target;

        for (int i = 0; i < data.recipes.Length; i++)
        {
            if (data.recipes[i] == null) continue;
            data.recipes[i].towerID = i + 1;
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(data);
        }
    }
}
