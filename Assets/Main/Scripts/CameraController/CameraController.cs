using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float zoomSpeed = 0.5f;
    public float zoomSmoothTime = 0.2f;
    public float panSpeed = 0.1f;
    public float velocityDamping = 0.1f;
    public float minZoom = 1f;
    public float maxZoom = 5f;
    public float minX = -10f;
    public float maxX = 10f;
    public float minY = -10f;
    public float maxY = 10f;
    public float stopThreshold = 0.05f;

    private Camera mainCamera;
    private Vector2 touchStart;
    private Vector3 velocity;
    private float targetZoom;
    private float zoomSmoothVelocity;

    private void Start()
    {
        mainCamera = Camera.main;
        targetZoom = mainCamera.orthographicSize;
    }

    void Update()
    {
        if (Input.touchSupported)
        {
            HandleTouchInput();
        }
        else
        {
            HandleMouseInput();
        }

        // Apply velocity damping
        velocity *= (1f - velocityDamping * Time.deltaTime);

        // Apply velocity to camera position
        Vector3 translation = velocity * Time.deltaTime;
        Vector3 newPos = mainCamera.transform.position + translation;
        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        newPos.y = Mathf.Clamp(newPos.y, minY, maxY);
        mainCamera.transform.Translate(translation, Space.World);
        mainCamera.transform.position = newPos;

        // Gradually decrease velocity below the threshold
        if (velocity.magnitude < stopThreshold)
        {
            velocity = Vector3.Lerp(velocity, Vector3.zero, velocityDamping * Time.deltaTime);
        }

        // Smoothly zoom the camera
        mainCamera.orthographicSize = Mathf.SmoothDamp(mainCamera.orthographicSize, targetZoom, ref zoomSmoothVelocity, zoomSmoothTime);
    }

    void HandleTouchInput()
    {
        if (Input.touchCount == 2)
        {
            // Handle pinch-to-zoom
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

            float prevMagnitude = (touch0PrevPos - touch1PrevPos).magnitude;
            float currentMagnitude = (touch0.position - touch1.position).magnitude;

            float zoomDifference = prevMagnitude - currentMagnitude;

            ZoomCamera(zoomDifference * zoomSpeed);
        }
        else if (Input.touchCount == 1)
        {
            // Handle camera panning
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchStart = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector2 direction = (Vector2)touchStart - touch.position;
                PanCamera(direction * panSpeed);
                touchStart = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                // Apply velocity in the direction of movement
                velocity = (Vector3)((Vector2)touch.position - touchStart) * panSpeed;
            }
        }
        else
        {
            // Gradually decrease velocity if no touch input
            velocity = Vector3.Lerp(velocity, Vector3.zero, velocityDamping * Time.deltaTime);
        }
    }

    void HandleMouseInput()
    {
        // Handle camera panning with left mouse button
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector2 direction = (Vector2)touchStart - (Vector2)Input.mousePosition;
            PanCamera(direction * panSpeed);
            touchStart = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // Apply velocity in the direction of movement
            velocity = (Vector3)((Vector2)Input.mousePosition - touchStart) * panSpeed;
        }

        // Handle zoom with mouse scroll wheel
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        ZoomCamera(scrollInput * zoomSpeed * 10f);
    }

    void ZoomCamera(float zoomAmount)
    {
        targetZoom = Mathf.Clamp(targetZoom - zoomAmount, minZoom, maxZoom);
    }

    void PanCamera(Vector2 panAmount)
    {
        Vector3 newPos = mainCamera.transform.position + (Vector3)panAmount;
        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        newPos.y = Mathf.Clamp(newPos.y, minY, maxY);
        velocity = (newPos - mainCamera.transform.position) * (1f - velocityDamping);
        mainCamera.transform.Translate((Vector3)panAmount, Space.World);
        mainCamera.transform.position = newPos;
    }
}
