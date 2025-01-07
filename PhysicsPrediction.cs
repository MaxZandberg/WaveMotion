using UnityEngine;

public class PhysicsPrediction : MonoBehaviour
{
    [Header("Hand Reference")]
    [Tooltip("Reference to the VR hand transform.")]
    public Transform handTransform;

    [Header("Prediction Settings")]
    [Tooltip("Time into the future to predict (in seconds).")]
    public float predictionTime = 0.3f;

    [Tooltip("Smoothing factor to reduce prediction jitter.")]
    public float smoothingFactor = 0.1f;

    private Vector3 previousPosition; // Stores hand's previous position
    private Vector3 currentVelocity; // Stores calculated velocity
    private Vector3 predictedPosition; // Stores predicted future position

    void Start()
    {
        if (handTransform == null)
        {
            Debug.LogError("hand not assigned.");
        }

        previousPosition = handTransform.position;
    }
   
    void Update()
    {
        if (handTransform == null) return;

        // Step 1: Calculate Velocity
        currentVelocity = (handTransform.position - previousPosition) / Time.deltaTime;
        previousPosition = handTransform.position;

        // Step 2: Predict Future Position
        predictedPosition = PredictTrajectory();

        // Step 3: Visualization
        Debug.DrawLine(handTransform.position, predictedPosition, Color.cyan); // Trajectory Line
        Debug.DrawRay(handTransform.position, currentVelocity, Color.yellow); // Velocity Line
    }
 
    private Vector3 PredictTrajectory()
    {
        // Using basic physics formula: P_future = P_current + V * t reminds me of AP Physics 1 :( When this eq used to be our biggest issue
        Vector3 futurePosition = handTransform.position
                               + currentVelocity * predictionTime;
        // Apply smoothing to reduce jumps
        futurePosition = Vector3.Lerp(handTransform.position, futurePosition, smoothingFactor);
        return futurePosition;
    }

    public Vector3 GetPredictedPosition()
    {
        return predictedPosition;
    }
    private void OnDrawGizmos()
    {
        if (handTransform == null) return;

        // Draw cur hand position
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(handTransform.position, 0.03f);

        // Draw predicted position
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(predictedPosition, 0.05f);
    }
}
