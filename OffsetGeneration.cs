using UnityEngine;
using System.Collections.Generic;
public class OffsetGeneration : MonoBehaviour
{
    [Header("Prediction Reference")]
    [Tooltip("Reference to PhysicsPrediction")]
    public PhysicsPrediction physicsPrediction;

    [Header("Offset Settings")]
    [Tooltip("Distance offset points")]
    public float offsetDistance = 0.1f;

    private Vector3 predictedPosition;
    private List<Vector3> offsetPoints = new List<Vector3>();

    void Update()
    {
        predictedPosition = physicsPrediction.GetPredictedPosition();
        GenerateOffsets();      
    }
    private void GenerateOffsets()
    {
        offsetPoints.Clear();
        offsetPoints.Add(predictedPosition + Vector3.right * offsetDistance);  // +X
        offsetPoints.Add(predictedPosition - Vector3.right * offsetDistance);  // -X
        offsetPoints.Add(predictedPosition + Vector3.up * offsetDistance);     // +Y
        offsetPoints.Add(predictedPosition - Vector3.up * offsetDistance);     // -Y
        offsetPoints.Add(predictedPosition + Vector3.forward * offsetDistance); // +Z
        offsetPoints.Add(predictedPosition - Vector3.forward * offsetDistance); // -Z
    }
    public List<Vector3> GetOffsetPoints()
    {
        return offsetPoints;
    }
    private void OnDrawGizmos()
    {
        if (physicsPrediction == null || offsetPoints == null || offsetPoints.Count == 0)
            return;
        //  predicted position
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(predictedPosition, 0.03f);
        //  offset points
        Gizmos.color = Color.yellow;
        foreach (Vector3 offset in offsetPoints)
        {
            Gizmos.DrawSphere(offset, 0.02f);
        }
    }
}
