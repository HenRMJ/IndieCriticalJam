using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(LevelSO))]
public class LevelSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Editor Tools", EditorStyles.boldLabel);

        GameObject selectedObject = Selection.activeGameObject;
        RectTransform selectedRect = selectedObject ? selectedObject.GetComponent<RectTransform>() : null;

        EditorGUI.BeginDisabledGroup(selectedRect == null);
        if (GUILayout.Button("Capture Current Transform"))
        {
            LevelSO prompt = (LevelSO)target;

            TransformValues newValues = new(
                selectedRect.anchoredPosition,
                selectedRect.localEulerAngles.z,
                selectedRect.localScale.x);

            List<TransformValues> listToReplace = prompt.SubLevel.ToList<TransformValues>();
            listToReplace.Add(newValues);
            prompt.SubLevel = listToReplace.ToArray();

            EditorUtility.SetDirty(prompt);
            AssetDatabase.SaveAssets();
        }
        EditorGUI.EndDisabledGroup();

        DrawDefaultInspector();        
    }
}
