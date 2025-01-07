using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class ObjectRecognition : MonoBehaviour
{
    [Header("Camera Reference")]
    public Camera vrCamera;

    [Header("Detection Settings")]
    public string interactiveTag = "Interactive";
    public float maxDetectionDistance = 10f;

    [Range(5.0f, 90.0f)]
    public float detectionFOV = 25.0f; // Custom FoV 

    private List<(GameObject obj, float distance)> detectedObjects = new List<(GameObject, float)>();

    void Start()// yay we dont need to start anything here
    {
    }

    void Update()
    {
        if (vrCamera == null)
        {
            Debug.LogError("VR Camera is not assigned!");
            return;
        }

        detectedObjects.Clear();

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag(interactiveTag)) // Grabs every obj with #
        {
            Renderer renderer = obj.GetComponent<Renderer>(); // Tries to render obj 
            if (renderer == null) continue; //if it cant, it skips to the next obj in foreach

           //Distance Check
            float distance = Vector3.Distance(vrCamera.transform.position, obj.transform.position);
            if (distance > maxDetectionDistance) continue;//same thing- If it bad, it gets skipped

            Vector3 directionToObject = (obj.transform.position - vrCamera.transform.position).normalized;
            float angleToObject = Vector3.Angle(vrCamera.transform.forward, directionToObject);

            if (angleToObject > detectionFOV / 2) // if angle is out of 1/2 of fov. The camera center -> obj creates angle kinda like this \|/. 
                Debug.DrawLine(vrCamera.transform.position, obj.transform.position, Color.red);
                continue;
            }

            Ray ray = new Ray(vrCamera.transform.position, directionToObject);

            if (Physics.Raycast(ray, out RaycastHit hit, maxDetectionDistance))
            {
                if (hit.collider.gameObject == obj)
                {
                    Debug.DrawLine(vrCamera.transform.position, obj.transform.position, Color.green);
                    detectedObjects.Add((obj, distance)); //if obj is proerly traced, added to list with distance
                }
                else
                {
                    Debug.DrawLine(vrCamera.transform.position, hit.point, Color.yellow); //blocked
                }
            }
            else
            {
                Debug.DrawLine(vrCamera.transform.position, obj.transform.position, Color.blue);
            }
        }

        // determine the closest object
        if (detectedObjects.Count > 0)
        {
            var closestObject = detectedObjects.OrderBy(detObj => detObj.distance).First();//Handy little one line that orders and gets closest obj
            Debug.DrawLine(vrCamera.transform.position, closestObject.obj.transform.position, Color.magenta, 0.1f);
            Debug.Log($"Closest Object: {closestObject.obj.name} (Distance: {closestObject.distance:F2})");
        }
    }
    public Transform GetClosestObject()
    {
        if (detectedObjects.Count == 0)
        {
            Debug.LogWarning("No detected objects found.");
            return null;
        }

        return detectedObjects.OrderBy(detObj => detObj.distance).First().obj.transform;
    }
    private void OnDrawGizmos()
    {
        if (vrCamera == null) return;

        Gizmos.color = Color.yellow;
        Vector3 leftBoundary = Quaternion.Euler(0, -detectionFOV / 2, 0) * vrCamera.transform.forward;//Again, does that little \|/ to trace the bounds 
        Vector3 rightBoundary = Quaternion.Euler(0, detectionFOV / 2, 0) * vrCamera.transform.forward;

        Gizmos.DrawLine(vrCamera.transform.position, vrCamera.transform.position + leftBoundary * maxDetectionDistance);
        Gizmos.DrawLine(vrCamera.transform.position, vrCamera.transform.position + rightBoundary * maxDetectionDistance);
        Gizmos.DrawLine(vrCamera.transform.position + leftBoundary * maxDetectionDistance, vrCamera.transform.position + rightBoundary * maxDetectionDistance);
    }
}
