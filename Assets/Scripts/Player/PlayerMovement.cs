using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;

    private Animator animator;
    private Rigidbody2D rb;
    private PlayerFacing facing;
    private Vector2 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        facing = GetComponent<PlayerFacing>();
    }
    public void IncreaseSpeed(float amount)
    {
        speed += amount;
    }
    void Update()
    {
        moveInput = Vector2.zero;

        if (Input.GetKey(KeyCode.W))
            moveInput.y += 1;
        if (Input.GetKey(KeyCode.S))
            moveInput.y -= 1;
        if (Input.GetKey(KeyCode.A))
            moveInput.x -= 1;
        if (Input.GetKey(KeyCode.D))
            moveInput.x += 1;

        moveInput = moveInput.normalized;

        animator.SetBool("isMoving", moveInput.sqrMagnitude > 0.01f);

        // REQUEST facing (do NOT flip here)
        if (moveInput.x != 0 && !facing.IsShooting)
        {
            facing.Face(moveInput.x);
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput * speed;
    }
}
