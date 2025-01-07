using UnityEngine;
using System.Collections.Generic;

public class IKValidation : MonoBehaviour
{
    [Header("Offset Ref")]
    [Tooltip("Ref to the OffsetGeneration")]
    public OffsetGeneration offsetGeneration;

    [Header("IK Settings")]
    [Tooltip("Ref to the Animator")]
    public Animator animator;

    [Tooltip("Hand Goal for IK")]
    public AvatarIKGoal handIKGoal = AvatarIKGoal.RightHand;

    private List<Vector3> validOffsets = new List<Vector3>();
    private List<Vector3> invalidOffsets = new List<Vector3>();

    void Update()
    {
        if (offsetGeneration == null || animator == null)
        {
            Debug.LogError("in update");
            return;
        }
        List<Vector3> offsetPoints = offsetGeneration.GetOffsetPoints();
        validOffsets.Clear();
        invalidOffsets.Clear();
        foreach (Vector3 point in offsetPoints)
        {
            if (IsReachable(point))
            {
                validOffsets.Add(point);
            }
            else
            {
                invalidOffsets.Add(point);
            }
        }
     //   VisualizeOffsets();
    }
   
    private bool IsReachable(Vector3 point)
    {
        animator.SetIKPositionWeight(handIKGoal, 1.0f);
        animator.SetIKPosition(handIKGoal, point);

        Vector3 adjustedPosition = animator.GetIKPosition(handIKGoal);
        float distance = Vector3.Distance(adjustedPosition, point);
        return distance < 0.05f;
    }
    public List<Vector3> GetValidOffsets()
    {
        return validOffsets;
    }

    private void OnDrawGizmos()
    {
        if (validOffsets == null || invalidOffsets == null)
            return;
        Gizmos.color = Color.green;
        foreach (Vector3 point in validOffsets)
        {
            Gizmos.DrawSphere(point, 0.02f);
        }

        Gizmos.color = Color.red;
        foreach (Vector3 point in invalidOffsets)
        {
            Gizmos.DrawSphere(point, 0.02f);
        }
    }
}
