using UnityEngine;
using UnityEngine.InputSystem;

// Attach this to an empty "CameraRig" GameObject, with your actual Camera
// as its CHILD, offset back and up at whatever tilt you want.
// WASD pans the rig, Q/E rotates its, scroll wheel dollies the camera in/out.


public class CameraController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The child Camera transform, offset from this rig's origin")]
    public Transform cameraTransform;

    [Header("Panning (WASD)")]
    public float panSpeed = 20f;

    [Header("Rotation (Q/E)")]
    public float rotationSpeed = 90f;  //degrees per second

    [Header("Zoom (scroll wheel)")]
    public float zoomSpeed = 5f;
    public float minZoomDistance = 5f;
    public float maxZoomDistance = 25f;

    private Vector3 _zoomDirection;     // normalized local offset direction of the camera child
    private float _currentZoomDistance; // current distance along that direction

    private void Start()
    {
        if (cameraTransform == null)
        {
            Debug.LogError("CameraController: assign the child Camera transform in the Inspector.");
            enabled = false;
            return;
        }

        _zoomDirection = cameraTransform.localPosition.normalized;
        _currentZoomDistance = cameraTransform.localPosition.magnitude;

        // If the camera's starting offset is outside Min/Max Zoom Distance, clamp it now
        // (in Start, not mid-scroll) so there's no sudden jump on the first scroll input,
        // and warn so you know to widen the range instead of just living with the clamp.
        float clamped = Mathf.Clamp(_currentZoomDistance, minZoomDistance, maxZoomDistance);
        if (!Mathf.Approximately(clamped, _currentZoomDistance))
        {
            Debug.LogWarning($"CameraController: starting camera distance ({_currentZoomDistance:F1}) is outside " +
                              $"Min/Max Zoom Distance ({minZoomDistance}-{maxZoomDistance}). Clamping now to avoid a jump. " +
                              "Consider widening Max Zoom Distance to match your intended default view.");
            _currentZoomDistance = clamped;
            cameraTransform.localPosition = _zoomDirection * _currentZoomDistance;
        }

    }

    private void Update()
    {
        HandlePan();
        HandleRotation();
        HandleZoom();
    }

    private void HandlePan()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        Vector3 input = Vector3.zero;
        if (kb.wKey.isPressed) input += Vector3.forward;
        if (kb.sKey.isPressed) input += Vector3.back;
        if (kb.aKey.isPressed) input += Vector3.left;
        if (kb.dKey.isPressed) input += Vector3.right;

        if (input == Vector3.zero) return;

        // Move relative to the rig's own facin, so panning stays intuitive after rotation with Q/E
        Vector3 move = transform.TransformDirection(input.normalized);
        move.y = 0f; // keep panning flat regardless of camera tilt
        transform.position += move * panSpeed * Time.deltaTime;
    }

    private void HandleRotation()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        float rotationInput = 0f;
        if (kb.qKey.isPressed) rotationInput -= 1f;
        if (kb.eKey.isPressed) rotationInput += 1f;

        if (rotationInput == 0f) return;

        transform.Rotate(Vector3.up, rotationInput * rotationSpeed * Time.deltaTime, Space.World);
    }

    private void HandleZoom()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null) return;

        float scroll = mouse.scroll.ReadValue().y;
        if (Mathf.Approximately(scroll, 0f)) return;


        // New Input System reports scroll in raw units of ~120 per notch/click, not a smooth
        // per-frame velocity. Normalize to "notches" so zoomSpeed means "units per scroll click" -
        // much easier to tune than the previous raw-scaled version.
        float notches = scroll / 120f;
        _currentZoomDistance -= notches * zoomSpeed;
        _currentZoomDistance = Mathf.Clamp(_currentZoomDistance, minZoomDistance, maxZoomDistance);

        cameraTransform.localPosition = _zoomDirection * _currentZoomDistance;

    }
}
