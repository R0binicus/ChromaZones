using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class FOVData
{
    [field:SerializeField, Range(0, 360)] public float FOVAngle { get; private set; }
    [field:SerializeField] public float FOVDist { get; private set; }
    [field:SerializeField] public uint TriangleSlices { get; private set; }

    public FOVData(float FOVAngle, float FOVDist, uint TriangleSlices)
    {
        this.FOVAngle = FOVAngle;
        this.FOVDist = FOVDist;
        this.TriangleSlices = TriangleSlices;
    }
}
