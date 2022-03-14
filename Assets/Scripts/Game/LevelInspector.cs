using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Level))]
public class LevelInspector : Editor
{
    public override void OnInspectorGUI()
    {
        Level level = (Level)target;

        level.levelIndex = EditorGUILayout.IntField("Level Index", level.levelIndex);
        level.levelPrefab = EditorGUILayout.ObjectField("Level Prefab To Spawn", level.levelPrefab,typeof(GameObject), true) as GameObject;
        

        EditorGUILayout.BeginHorizontal("Box");
            if(GUILayout.Button("Create Level"))
            {
                level.CreateLevel();
            }

            if(GUILayout.Button("Destroy Level"))
            {
                level.DestroyLevel();
            }
        EditorGUILayout.EndHorizontal();

    }
}
