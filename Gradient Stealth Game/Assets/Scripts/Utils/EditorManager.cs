#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyPatrollerBehaviour))]
public class EditorManager : Editor
{
	void DrawArrow(Vector3 start, Vector3 end)
	{
		if (Vector3.Distance(start, end) < 0.01) return;

		float head_size = 0.5f;
		float angle = 22.5f / 180f * Mathf.PI;

		float rx = Mathf.Cos(angle);
		float ry = Mathf.Sin(angle);

		Vector3 head = (start - end).normalized * head_size;

		Vector3 head1 = end + new Vector3(head.x * rx - head.y * ry, head.x *  ry + head.y * rx);
		Vector3 head2 = end + new Vector3(head.x * rx + head.y * ry, head.x * -ry + head.y * rx);

		Handles.DrawPolyLine(new Vector3[]{ start, end, head1, head2, end });
	}
	
	void OnSceneGUI()
	{
		EnemyPatrollerBehaviour m = target as EnemyPatrollerBehaviour;

		if (m == null)
			return;

		for (var i = 0; i < m.waypoints.Count; i++)
        {
			EditorGUI.BeginChangeCheck();
			Vector3 newRightEnd = Handles.PositionHandle(m.waypoints[i], Quaternion.identity);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(m, "Changed Right End Position");
				m.waypoints[i] = newRightEnd;
			}

			Handles.color = Color.green;
			if (i == 0)
			{
				DrawArrow(m.transform.position, m.waypoints[i]);
			}
			else
			{
				DrawArrow(m.waypoints[i-1], m.waypoints[i]);
			}
			
        }
		DrawArrow(m.waypoints[m.waypoints.Count-1], m.transform.position);
	}
}

#endif