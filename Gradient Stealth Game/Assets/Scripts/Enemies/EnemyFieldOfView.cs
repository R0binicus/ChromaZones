using UnityEngine;

public class EnemyFieldOfView : MonoBehaviour
{
    [Header("General Data")]
    [SerializeField] LayerMask _layerToRaycast;

    [Header("Patrol Data")]
    [SerializeField, Range(0, 360)] float _patrolFOVAngle;
    [SerializeField] float _patrolFOVDist;
    [SerializeField] uint _patrolTriangleSlices;

    [Header("Alert Data")]
    [SerializeField, Range(0, 360)] float _alertFOVAngle;
    [SerializeField] float _alertFOVDist;
    [SerializeField] uint _alertTriangleSlices;

    // Internal Data
    float _FOVAngle;
    float _FOVDist;
    uint _triangleSlices;

    public bool PlayerSpotted { get; private set; }

    private void FixedUpdate()
    {
        float currentAngle = GetAngleFromVectorFloat(transform.up) + (_FOVAngle / 2f); // Get starting angle first
        float angleIncrease = _FOVAngle / _triangleSlices; // Calculate how much to increase angle by

        for (int i = 0; i <= _triangleSlices; i++)
        {
            RaycastHit2D ray = Physics2D.Raycast(transform.position, GetVectorFromAngle(currentAngle), _FOVDist, _layerToRaycast);

            // Hit player
            if (ray.collider != null)
            {
                PlayerSpotted = true;
                break;
            }
            else
            {
                PlayerSpotted = false;
            }

            currentAngle -= angleIncrease; // Increase angle to check next ray
        }
    }

    // Creating a custom mesh for the field of view
    public void CreateFOV()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        Vector3[] vertices = new Vector3[_triangleSlices + 2]; // 1+ vertex for origin, 1+ to complete the last triangle
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[_triangleSlices * 3];

        vertices[0] = Vector3.zero; // Origin point

        float currentAngle = GetAngleFromVectorFloat(transform.up) + (_FOVAngle / 2f);
        float angleIncrease = _FOVAngle / _triangleSlices;

        int vertexIndex = 1;
        int triangleIndex = 0;

        // Create FOV mesh by creating each triangle
        for (int i = 0; i <= _triangleSlices; i++)
        {
            Vector3 vertex = vertices[0] + GetVectorFromAngle(currentAngle) * _FOVDist;
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

    public void SetPatrolFOVData()
    {
        _FOVAngle = _patrolFOVAngle;
        _FOVDist = _patrolFOVDist;
        _triangleSlices = _patrolTriangleSlices;
    }

    public void SetAlertFOVData()
    {
        _FOVAngle = _alertFOVAngle;
        _FOVDist = _alertFOVDist;
        _triangleSlices = _alertTriangleSlices;
    }

    public void IsActive(bool flag)
    {
        gameObject.SetActive(flag);
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

        if (n < 0)
        {
            n += 360;
        }

        return n;
    }
}
