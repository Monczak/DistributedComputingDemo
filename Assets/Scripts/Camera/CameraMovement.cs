using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    public float moveSpeed;
    public float moveSmoothing, coastSmoothing;
    public float maxSpeed;
    private Vector3 velocity;
    private float velocitySmoothing;
    private Vector3 currentVelocity;
    private Vector3 targetVelocity;

    private Controls controls;
    private Vector2 moveInput;
    private Vector2 previousMoveInput;

    private CameraController controller;

    private bool enableMovement;
    private bool previousEnableMovement;

    public bool fixInPlace;

    public float restrictSpeed;

    public bool panDown;
    public float panSpeed;
    private Vector3 panSmoothVelocity;

    public float zoomSpeed;
    public float zoomSmoothing;
    public float minRadius, maxRadius;
    private int zoomInput;
    private float zoomVelocity;

    private float currentRadius;
    private float targetRadius;

    private float moveSpeedRatio;
    private float defaultRadius;

    private void Awake()
    {
        controller = GetComponent<CameraController>();
        controls = new Controls();

        controls.Camera.Move.performed += OnMouseMove;
        controls.Camera.Move.canceled += OnMouseStop;
        controls.Camera.StartMovement.performed += OnStartMovement;
        controls.Camera.StartMovement.canceled += OnCancelMovement;
        controls.Camera.Zoom.performed += OnZoom;
        controls.Camera.Enable();

        velocitySmoothing = coastSmoothing;
        currentVelocity = Vector2.zero;

        currentRadius = controller.radius;
        targetRadius = currentRadius;
        defaultRadius = controller.radius;
    }

    private void OnZoom(InputAction.CallbackContext obj)
    {
        zoomInput = (int)Mathf.Sign(obj.ReadValue<float>());
    }

    private void OnMouseStop(InputAction.CallbackContext obj)
    {
        moveInput = Vector2.zero;
    }

    private void OnCancelMovement(InputAction.CallbackContext obj)
    {
        enableMovement = false;
        velocitySmoothing = coastSmoothing;
    }

    private void OnStartMovement(InputAction.CallbackContext obj)
    {
        enableMovement = true;
        velocitySmoothing = moveSmoothing;
    }

    private void OnMouseMove(InputAction.CallbackContext obj)
    {
        moveInput = obj.ReadValue<Vector2>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!fixInPlace)
            ProcessMouseInput();

        controller.position += currentVelocity * Time.smoothDeltaTime;
        currentVelocity = Vector3.SmoothDamp(currentVelocity, targetVelocity, ref velocity, velocitySmoothing);
        currentRadius = Mathf.SmoothDamp(currentRadius, targetRadius, ref zoomVelocity, zoomSmoothing);

        controller.radius = currentRadius;

        RestrictMovement();

        previousEnableMovement = enableMovement;
        previousMoveInput = moveInput;

        PanDown();
    }

    void ProcessMouseInput()
    {
        moveSpeedRatio = currentRadius / defaultRadius;

        if (enableMovement || previousEnableMovement)
        {
            Vector2 input = !enableMovement && previousEnableMovement && Mathf.Approximately(moveInput.sqrMagnitude, 0) ? previousMoveInput : moveInput;

            currentVelocity = moveSpeed * moveSpeedRatio * -new Vector3(input.x, 0, input.y) / Time.smoothDeltaTime;
            targetVelocity = currentVelocity;
        }
        else
            targetVelocity = Vector2.zero;

        if (zoomInput != 0)
        {
            targetRadius *= Mathf.Pow(zoomSpeed, -zoomInput);
            targetRadius = Mathf.Clamp(targetRadius, minRadius, maxRadius);
            zoomInput = 0;
        }
    }

    void PanDown()
    {
        controller.offset = Vector3.SmoothDamp(controller.offset, panDown ? Vector3.back * CameraUtility.GetScreenHeightInUnits() : Vector3.zero, ref panSmoothVelocity, panSpeed);
    }

    void RestrictMovement()
    {

    }

    public void SetZoomOutRadius(float radius)
    {
        targetRadius = radius;
        maxRadius = radius;
    }

    public void ResetPosition()
    {
        controller.position = Vector3.zero;
    }
}
