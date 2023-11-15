using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class EnemyFieldOfView : MonoBehaviour
{
    [SerializeField, Range(0, 360)] float _fovAngle;
    [SerializeField] float _fovDist;
    [SerializeField] uint _triangleSlices;

    void Start()
    {
        // Creating a custom mesh for the field of view
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        Vector3[] vertices = new Vector3[_triangleSlices + 2]; // 1+ vertex for origin, 1+ to complete the last triangle
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[_triangleSlices * 3];

        vertices[0] = Vector3.zero; // Origin point

        float currentAngle = GetAngleFromVectorFloat(transform.up) + (_fovAngle / 2f);
        float angleIncrease = _fovAngle / _triangleSlices;

        int vertexIndex = 1;
        int triangleIndex = 0;

        // Create FOV mesh by creating each triangle
        for (int i = 0; i <= _triangleSlices; i++)
        {
            Vector3 vertex = vertices[0] + GetVectorFromAngle(currentAngle) * _fovDist;
            vertices[vertexIndex] = vertex;

            if (i > 0)
            {
                triangles[triangleIndex] = 0; // Start drawing triangle at origin point
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3; // Go to next set of indexes for next triangle
            }

            vertexIndex++;
            currentAngle -= angleIncrease;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }

    Vector3 GetVectorFromAngle(float angle)
    {
        float rad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad));
    }

    float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        if (n < 0 )
        {
            n += 360;
        }

        return n;
    }
}
