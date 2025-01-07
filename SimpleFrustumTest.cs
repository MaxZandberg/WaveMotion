using UnityEngine;

/// <summary>
/// A minimal frustum visualization test.
/// </summary>
public class SimpleFrustumTest : MonoBehaviour
{
    public Camera vrCamera;

    private void OnDrawGizmos()
    {
        if (vrCamera == null)
        {
            Debug.LogError("VR Camera not assigned!");
            return;
        }

        Gizmos.color = Color.cyan;
        Gizmos.matrix = vrCamera.transform.localToWorldMatrix;
        Gizmos.DrawFrustum(Vector3.zero, vrCamera.fieldOfView, vrCamera.farClipPlane, vrCamera.nearClipPlane, vrCamera.aspect);
    }
}
