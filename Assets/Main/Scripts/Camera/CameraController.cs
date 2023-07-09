using Main.Scripts.Util;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Properties")]
    public Camera Camera;
    public Transform CameraTransform; 
    
    [Header("Attributes")]
    public float movementSpeed = 5f;
    public float zoomSpeed = 5f;
    public float maxZoom = 10f;
    public float minZoom = 2f;
    public float zoomDampening = 10f;
    public float maxPan = 10f;
    public float panDampening = 5f;
    public float bufferTime = 0.2f;
    public float maxMouseDeltaMagnitude = 10f;
    public float decelerationDuration = 0.5f;

    private float currentZoom = 5f;
    private Vector3 initialPosition;
    private Vector3 lastMousePosition;
    private Vector3 velocity;
    private bool isMoving = false;
    private float speedBufferTimer = 0f;
    private float decelerationTimer = 0f;
    private bool hitObject = false;

    private void Start()
    {
        initialPosition = CameraTransform.position;
        lastMousePosition = Input.mousePosition;
    }

    private void Update()
    {
        // Handle camera movement buffer
        if (isMoving)
        {
            speedBufferTimer += Time.deltaTime;
            if (speedBufferTimer >= bufferTime)
            {
                speedBufferTimer = bufferTime;
            }
        }
        else
        {
            speedBufferTimer = 0f;
        }

        // Handle camera movement
        Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
        Vector3 clampedMouseDelta = Vector3.ClampMagnitude(mouseDelta, maxMouseDeltaMagnitude);
        Vector3 desiredPosition = CameraTransform.position;

        if (Input.GetMouseButtonDown(0))
        {
            // Reset velocity and timers when the mouse button is clicked
            velocity = Vector3.zero;
            decelerationTimer = 0f;

            // Perform raycast to check for objects
            var mousePos = Camera.ScreenToWorldPoint(Input.mousePosition);
            var layer = LayerMask.NameToLayer(Constants.k_layer_matchable_object);
            if (Physics2D.Raycast(mousePos, Vector2.zero, 1000,
                    1 << layer))
            {
                // An object was hit, so don't move the camera
                hitObject = true;
                return;
            }
            else
            {
                hitObject = false;
            }
        }

        if (!hitObject && Input.GetMouseButton(0))
        {
            // Calculate velocity based on mouse movement and accumulated movement distance
            float acceleration = Mathf.Lerp(0f, movementSpeed, speedBufferTimer / bufferTime);
            velocity += new Vector3(-clampedMouseDelta.x, -clampedMouseDelta.y, 0f) * acceleration * Time.deltaTime;
            velocity = Vector3.ClampMagnitude(velocity, maxPan);
            isMoving = true;
            decelerationTimer = 0f;
        }

        // Apply damping to velocity
        velocity /= 1 + panDampening * Time.deltaTime;

        // Apply velocity to desired position
        desiredPosition += velocity;

        // Handle camera zoom
        float scrollDelta = Input.mouseScrollDelta.y;
        currentZoom -= scrollDelta * zoomSpeed * Time.deltaTime;

        // Clamp zoom level
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        // Apply damping to zoom
        currentZoom = Mathf.Lerp(currentZoom, currentZoom + scrollDelta, 1f / (1f + zoomDampening * Time.deltaTime));

        // Update camera position and zoom level
        var position = CameraTransform.position;
        position = desiredPosition;
        position = new Vector3(
            Mathf.Clamp(position.x, -maxPan, maxPan),
            Mathf.Clamp(position.y, -maxPan, maxPan),
            initialPosition.z);
        CameraTransform.position = position;
        Camera.orthographicSize = currentZoom;

        // Store current mouse position
        lastMousePosition = Input.mousePosition;

        // Handle deceleration
        if (!Input.GetMouseButton(0))
        {
            decelerationTimer += Time.deltaTime;
            if (decelerationTimer >= decelerationDuration)
            {
                velocity = Vector3.Lerp(velocity, Vector3.zero, panDampening * Time.deltaTime);
                isMoving = false;
            }
        }
    }
}
