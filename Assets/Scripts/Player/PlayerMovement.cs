using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 2.5f;

    CharacterController controller;
    Animator animator;
    float fall;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // read keys
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(x, 0f, z);

        // face the way she's walking
        if (direction != Vector3.zero)
        {
            transform.forward = direction;
        }

        // gravity
        fall = fall - 9.8f * Time.deltaTime;
        if (controller.isGrounded)
        {
            fall = -1f;
        }

        // move
        Vector3 move = direction * moveSpeed;
        move.y = fall;
        controller.Move(move * Time.deltaTime);

        // animation
        animator.SetFloat("Speed", direction.magnitude);

        //jump

        if(Input.GetKeyDown(KeyCode.Space))
        {
            
        }


    }
}