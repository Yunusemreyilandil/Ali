using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonCharacterController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float rotationSpeed = 10f;
    public float gravity = -9.81f;
    public float jumpForce = 8f;
    public Transform cameraTransform;
    public Animator animator;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isWalking = false;
    private bool isRunning = false;
    private bool isJumping = false;
    private bool isGrounded = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Yere temas kontrolü
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Küçük bir sabit deðerle yere sabitle
            isJumping = false;
            animator.SetBool("isJumping", false);
        }

        // Shift'e basýldýðýnda koþma
        isRunning = Input.GetKey(KeyCode.LeftShift);

        // Hareket hýzý seçimi
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        // Hareket Kontrolü
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // Yürüme ve koþma kontrolü
        isWalking = direction.magnitude >= 0.1f;
        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isRunning", isRunning);

        if (isWalking)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationSpeed, 0.1f);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * currentSpeed * Time.deltaTime);
        }

        // Zýplama kontrolü
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = jumpForce;
            isJumping = true;
            animator.SetBool("isJumping", true);
        }

        // Yerçekimi uygulama
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
