using UnityEngine;

// Moves the player with WASD relative to the camera, applies gravity,
// turns her to face the direction she's walking, and tells the Animator how fast she's going.
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 1.5f;   // walking speed in meters per second
    public float turnSpeed = 10f;    // how quickly she rotates to face her direction
    public float gravity = -9.8f;    // negative = pulls down

    [Header("Debug")]
    public bool logGrounded = false; // tick this in the Inspector to print isGrounded to the Console

    CharacterController controller;
    Animator animator;
    Transform cam;                   // NEW: the camera, so W/S/A/D can be relative to it
    float verticalVelocity;          // her up/down speed, used for falling

    void Start()
    {
        // Grab the components this script uses every frame
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();   // finds the Animator on her or on a child
        cam = Camera.main.transform;                     // NEW: the camera tagged "MainCamera"
    }

    void Update()
    {
        // 1. Read input: A/D gives x, W/S gives z, each between -1 and 1
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // 2. NEW: get the camera's forward and right directions, flattened onto the ground
        Vector3 camForward = cam.forward;
        camForward.y = 0f;           // ignore the camera looking up or down
        camForward.Normalize();      // length back to 1

        Vector3 camRight = cam.right;
        camRight.y = 0f;
        camRight.Normalize();

        // NEW: W/S moves along the camera's forward, A/D along the camera's right
        Vector3 direction = camForward * z + camRight * x;
        direction = Vector3.ClampMagnitude(direction, 1f);   // diagonals aren't faster

        // 3. Turn to face the direction she's moving (smoothly, not instantly)
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        // 4. Gravity: on the ground, press down gently; in the air, fall faster each frame
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        // (Your jump goes here: after gravity, before Move.)

        // 5. Move: sideways/forward movement plus the up/down falling speed, in one call
        Vector3 velocity = direction * moveSpeed;
        velocity.y = verticalVelocity;
        controller.Move(velocity * Time.deltaTime);

        // 6. Tell the Animator how fast she's moving (0 = Idle, above 0.1 = Walk)
        if (animator != null)
        {
            animator.SetFloat("Speed", direction.magnitude);
        }

        if (logGrounded)
        {
            Debug.Log("Grounded: " + controller.isGrounded);
        }
    }
}