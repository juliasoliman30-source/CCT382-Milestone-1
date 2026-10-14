using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class AstronautController : MonoBehaviour
{
    [SerializeField] InputActionAsset inputActions;   

    [SerializeField] float walkSpeed = 2f;
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float turnSpeed = 120f;          
    [SerializeField] float jumpHeight = 1.2f;
    [SerializeField] float gravity = -20f;

    CharacterController cc;
    Animator animator;
    InputAction move, sprint, jump;
    float speed;
    float verticalVelocity;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        InputActionMap map = inputActions.FindActionMap("Player");
        move = map.FindAction("Move");
        sprint = map.FindAction("Sprint");
        jump = map.FindAction("Jump");

        Cursor.lockState = CursorLockMode.Locked;    
    }

    void OnEnable() => inputActions.Enable();
    void OnDisable() => inputActions.Disable();

    void Update()
    {
        float dt = Time.deltaTime;
        Vector2 input = move.ReadValue<Vector2>();

        transform.Rotate(0f, input.x * turnSpeed * dt, 0f);

        float topSpeed = sprint.IsPressed() ? runSpeed : walkSpeed;
        speed = Mathf.MoveTowards(speed, input.y * topSpeed, 12f * dt);

        if (cc.isGrounded)
        {
            verticalVelocity = -2f;
            if (jump.WasPressedThisFrame())
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                animator.SetTrigger("Jump");
            }
        }
        verticalVelocity += gravity * dt;

        cc.Move((transform.forward * speed + Vector3.up * verticalVelocity) * dt);

        animator.SetFloat("Speed", Mathf.Abs(speed), 0.1f, dt);
        animator.SetBool("Grounded", cc.isGrounded);
    }
}
