using UnityEngine;
using VContainer;

[RequireComponent(typeof(CharacterController))]
public class BikeController : MonoBehaviour
{
    [Inject] private GameManager gameManager;

    [Header("속도")]
    public float maxSpeed = 20f;
    public float acceleration = 5f;
    public float brakeForce = 10f;

    [Header("조향")]
    public float steerSpeed = 80f;

    [Header("카메라")]
    public float mouseSensitivity = 2f;
    public float maxLookUpAngle = 30f;
    public float maxLookDownAngle = 60f;

    [Header("물리")]
    public float gravity = -20f;

    private CharacterController controller;
    private float currentSpeed;
    private float verticalVelocity;
    private float cameraPitch;
    private Transform cameraTransform;
    private bool isActive;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cameraTransform = GetComponentInChildren<Camera>()?.transform;

        gameManager.OnGameStateChanged += OnGameStateChanged;
        SetActive(false);
    }

    void OnDestroy()
    {
        if (gameManager != null)
            gameManager.OnGameStateChanged -= OnGameStateChanged;
    }

    void OnGameStateChanged(GameManager.GameState state)
    {
        SetActive(state == GameManager.GameState.Playing);
    }

    void SetActive(bool active)
    {
        isActive = active;
        if (active)
        {
            currentSpeed = 0f;
            cameraPitch = 0f;
        }
    }

    void Update()
    {
        if (!isActive) return;

        HandleSteering();
        HandleMovement();
        HandleCamera();
    }

    void HandleSteering()
    {
        float steer = Input.GetAxis("Horizontal");
        if (Mathf.Abs(currentSpeed) > 0.5f)
        {
            float turnAmount = steer * steerSpeed * Time.deltaTime;
            turnAmount *= Mathf.Clamp01(Mathf.Abs(currentSpeed) / 5f);
            transform.Rotate(0, turnAmount, 0);
        }
    }

    void HandleMovement()
    {
        float input = Input.GetAxis("Vertical");

        if (input > 0)
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.deltaTime);
        else if (input < 0)
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, brakeForce * Time.deltaTime);
        else
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, acceleration * 0.3f * Time.deltaTime);

        if (controller.isGrounded)
            verticalVelocity = -2f;
        else
            verticalVelocity += gravity * Time.deltaTime;

        Vector3 move = transform.forward * currentSpeed;
        move.y = verticalVelocity;
        controller.Move(move * Time.deltaTime);
    }

    void HandleCamera()
    {
        if (cameraTransform == null) return;

        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -maxLookUpAngle, maxLookDownAngle);
        cameraTransform.localEulerAngles = new Vector3(cameraPitch, 0, 0);
    }

    public void PushBackToRoad(Vector3 roadCenter, float roadHalfWidth)
    {
        Vector3 toCenter = roadCenter - transform.position;
        toCenter.y = 0;
        float dist = toCenter.magnitude;

        if (dist > roadHalfWidth)
        {
            Vector3 pushDir = toCenter.normalized;
            controller.Move(pushDir * (dist - roadHalfWidth + 0.1f));
            currentSpeed *= 0.5f;
        }
    }
}
