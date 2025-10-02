using UnityEngine;

public class SphereDragHandler : MonoBehaviour
{
    private bool _isDragging = false;
    private Vector2 _initialTouchPosition;
    private Vector3 _initialWorldPosition;

    void Update()
    {
        // Check for touch input using Unity's built-in touch system
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                // Check if touch is over our sphere
                if (IsTouchOverSphere(touch.position))
                {
                    _isDragging = true;
                    _initialTouchPosition = touch.position;
                    _initialWorldPosition = transform.position;

                    // Make sphere kinematic during drag
                    Rigidbody rb = GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.isKinematic = true;
                        rb.useGravity = false;
                    }
                }
            }
            else if (touch.phase == TouchPhase.Moved && _isDragging)
            {
                // Update drag position
                HandleDrag(touch.position);
            }
            else if (touch.phase == TouchPhase.Ended && _isDragging)
            {
                // End drag and launch
                _isDragging = false;
                LaunchSphere(_initialTouchPosition, touch.position);
            }
        }
        else if (_isDragging)
        {
            // Handle case where touch ended without TouchPhase.Ended
            _isDragging = false;
            LaunchSphere(_initialTouchPosition, _initialTouchPosition);
        }
    }

    private bool IsTouchOverSphere(Vector2 screenPosition)
    {
        // Convert screen position to ray
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);

        // Check if ray hits our sphere
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            return hit.collider.gameObject == gameObject;
        }

        return false;
    }

    private void HandleDrag(Vector2 currentTouchPosition)
    {
        // Convert screen position to world position at plane height
        Vector3 worldPosition = CalculateWorldPositionFromTouch(currentTouchPosition);
        transform.position = worldPosition;
    }

    private Vector3 CalculateWorldPositionFromTouch(Vector2 screenPoint)
    {
        // Create a ray from camera through touch point
        Ray ray = Camera.main.ScreenPointToRay(screenPoint);

        // Check if we hit any detected planes
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Position the sphere at the hit point with slight offset above the plane
            return new Vector3(hit.point.x, hit.point.y + 0.1f, hit.point.z);
        }

        // Fallback position if no plane is detected
        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0;
        return Camera.main.transform.position + cameraForward * 1.5f;
    }

    private void LaunchSphere(Vector2 initialTouch, Vector2 finalTouch,
                             float minDragThreshold = 50f, float forceMultiplier = 0.15f,
                             float upwardForceRatio = 0.2f)
    {
        // Calculate the drag vector in screen space
        Vector2 dragVector = initialTouch - finalTouch;
        float dragDistance = dragVector.magnitude;

        // Only launch if drag exceeds minimum threshold to prevent accidental launches
        if (dragDistance < minDragThreshold)
        {
            Debug.Log("Drag below threshold - no launch");
            ResetSpherePosition();
            return;
        }

        // Calculate horizontal direction based on drag
        Vector3 horizontalDirection = new Vector3(dragVector.x, 0, dragVector.y).normalized;

        // Convert to world direction using camera's transform
        Vector3 launchDirection = Camera.main.transform.TransformDirection(horizontalDirection);

        // Apply slight upward angle for more natural trajectory
        launchDirection += Vector3.up * upwardForceRatio;
        launchDirection = launchDirection.normalized;

        // Calculate final force based on drag distance
        float launchForce = dragDistance * forceMultiplier;

        // Get reference to Rigidbody component
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            Debug.LogWarning("Rigidbody was missing - added dynamically");
        }

        // Prepare sphere for physics simulation
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Apply force with impulse mode for instant launch effect
        rb.AddForce(launchDirection * launchForce, ForceMode.Impulse);

        // Optional: Add slight spin for more realistic motion
        rb.AddTorque(
            new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) *
            (launchForce * 0.1f),
            ForceMode.Impulse
        );

        Debug.Log($"Sphere launched with force: {launchForce:F2}, direction: {launchDirection}");
    }

    /// <summary>
    /// Resets the sphere to its original position if drag was too small
    /// </summary>
    private void ResetSpherePosition()
    {
        transform.position = Vector3.zero;
    }
}
