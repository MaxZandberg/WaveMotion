using UnityEngine;
using System.Collections.Generic;
using System.IO;


public class InteractionLogic : MonoBehaviour
{
    [Header("Validation Reference")]
    public IKValidation ikValidation;

    [Header("Proxy Reference")]
    public Transform handProxy; // Proxy object

    [Header("Hand Reference")]
    public Transform vrHandTransform; // VR hand/controller position

    [Header("Object Recognition Reference")]
    public ObjectRecognition objectRecognition;

    [Header("Blending Settings")]
    public float proxyBlendSpeed = 3.0f; // Speed for proxy movement
    public float interactionThreshold = 0.05f; // Distance to trigger interaction

    private Vector3 targetPosition;
    private Transform detectedObject;
    private bool interactionTriggered = false;

    [Header("Data Logging")]
    public string dataFileName = "InteractionData.csv";
    private List<string> dataLog = new List<string>();
    private float detectionTime;
    private float proxyStartTime;
    private float interactionTime;

    void Start()
    {
        InitializeDataFile()

        if (handProxy != null && vrHandTransform != null)
        {
            handProxy.position = vrHandTransform.position;
        }
    }

    void Update()
    {
        if (ikValidation == null || handProxy == null || vrHandTransform == null || objectRecognition == null)
        {
            Debug.LogError("Missing ref in update");
            return;
        }

        DetectAndUpdateTarget();
        GuideProxyTowardsObject();
        CheckInteraction();
    }
    private void DetectAndUpdateTarget()
    {
        Transform newDetectedObject = objectRecognition.GetClosestObject();

        if (newDetectedObject == null)
        {
            Debug.LogWarning("no obj detected");
            return;
        }

        // Update target if a new object is detected
        if (detectedObject != newDetectedObject)
        {
            detectedObject = newDetectedObject;
            targetPosition = detectedObject.position;
            detectionTime = Time.time;
            proxyStartTime = 0;

        }
    }
    private void GuideProxyTowardsObject()
    {
        if (targetPosition == Vector3.zero || detectedObject == null) return;

        // calculate direction from the hand to the target object
        Vector3 directionToTarget = (targetPosition - vrHandTransform.position).normalized;
        float distanceToTarget = Vector3.Distance(vrHandTransform.position, targetPosition);
        Vector3 intermediateTarget = vrHandTransform.position + (directionToTarget * (distanceToTarget * 0.5f));//gets the intermediate target to smooth to 
        handProxy.position = Vector3.Lerp(handProxy.position,intermediateTarget, proxyBlendSpeed * Time.deltaTime
        );

        // Debug visualization
        Debug.DrawLine(vrHandTransform.position, handProxy.position, Color.yellow);
        Debug.DrawLine(handProxy.position, targetPosition, Color.green); 
    }

    private void CheckInteraction()
    {
        float proxyToTargetDistance = Vector3.Distance(handProxy.position, targetPosition);
        float handToProxyDistance = Vector3.Distance(vrHandTransform.position, handProxy.position);

        // if interaction threshold is met
        if (proxyToTargetDistance < interactionThreshold && !interactionTriggered)
        {
            interactionTriggered = true;
            interactionTime = Time.time - detectionTime;

            RecordInteractionData(proxyToTargetDistance, handToProxyDistance, interactionTime);
            TriggerInteraction();
        }

        // Reset interaction trigger if hand moves away... this kinda fixes the data collection, you just have to move your hand quickly away. 
        if (interactionTriggered && handToProxyDistance > interactionThreshold * 1.5f)
        {
            interactionTriggered = false;
            Debug.Log("Interaction Reset: Ready for next trigger.");
        }
    }
    private void TriggerInteraction()
    {
        Debug.Log("Interaction Triggered at Optimal Offset!");
    }
    private void RecordInteractionData(float proxyError, float finalDistance, float timeElapsed)
    {
        string dataEntry = $"{Time.time},{targetPosition.x},{targetPosition.y},{targetPosition.z},{proxyError},{finalDistance},{timeElapsed}";
        dataLog.Add(dataEntry);

        Debug.Log($"Data Recorded: {dataEntry}");
    }
    private void InitializeDataFile()
    {
        string header = "Timestamp,TargetX,TargetY,TargetZ,ProxyError,FinalDistance,TimeElapsed";//future obj for implementation just incase
        dataLog.Add(header);
    }
    private void OnApplicationQuit()
    {
        string filePath = Path.Combine(Application.persistentDataPath, dataFileName);
        File.WriteAllLines(filePath, dataLog);
        Debug.Log($"Data saved to {filePath}");
    }
}
