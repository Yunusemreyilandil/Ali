using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonCharacterController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float gravity = -9.81f;
    public float jumpForce = 8f;
    public float crouchHeight = 1f;
    public float standingHeight = 2f;
    public float crouchCenterY = 0.5f;
    public float standingCenterY = 1f;
    public Transform cameraTransform;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isCrouching = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        // Baþlangýçta ayakta baþla
        controller.height = standingHeight;
        controller.center = new Vector3(0, standingCenterY, 0);
    }

    void Update()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationSpeed, 0.1f);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * moveSpeed * Time.deltaTime);
        }

        // Zýplama kontrolü
        if (isGrounded && Input.GetButtonDown("Jump") && !isCrouching)
        {
            velocity.y = jumpForce;
        }

        // Eðilme kontrolü (CTRL tuþu)
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = true;
            controller.height = crouchHeight;
            controller.center = new Vector3(0, crouchCenterY, 0);
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            // Üstte engel yoksa kalk
            if (CanStandUp())
            {
                isCrouching = false;
                controller.height = standingHeight;
                controller.center = new Vector3(0, standingCenterY, 0);
            }
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // Karakterin üstünde engel var mý kontrolü (kalkýþ için)
    bool CanStandUp()
    {
        float checkDistance = standingHeight - crouchHeight;
        Vector3 start = transform.position + Vector3.up * crouchHeight;
        return !Physics.SphereCast(start, controller.radius * 0.95f, Vector3.up, out RaycastHit hit, checkDistance, ~0, QueryTriggerInteraction.Ignore);
    }
}