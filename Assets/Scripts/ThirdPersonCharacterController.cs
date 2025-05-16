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
        // Yere temas kontrol�
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // K���k bir sabit de�erle yere sabitle
            isJumping = false;
            animator.SetBool("isJumping", false);
        }

        // Shift'e bas�ld���nda ko�ma
        isRunning = Input.GetKey(KeyCode.LeftShift);

        // Hareket h�z� se�imi
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        // Hareket Kontrol�
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // Y�r�me ve ko�ma kontrol�
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

        // Z�plama kontrol�
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = jumpForce;
            isJumping = true;
            animator.SetBool("isJumping", true);
        }

        // Yer�ekimi uygulama
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
