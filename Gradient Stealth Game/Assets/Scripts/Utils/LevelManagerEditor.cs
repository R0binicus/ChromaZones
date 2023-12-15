#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelManager))]
public class LevelManagerEditor : Editor
{
    protected virtual void OnSceneGUI()
	{
		LevelManager levelManager = (LevelManager)target;

        if (levelManager == null)
        {
            Debug.LogError("LevelManager null for editor");
        }

        EditorGUI.BeginChangeCheck();
        Vector3 newTargetPosition = Handles.PositionHandle(levelManager.PlayerSpawn, Quaternion.identity);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(levelManager, "Changed Player Spawnpoint");
            levelManager.PlayerSpawn = newTargetPosition;
        }
    }
}

#endif