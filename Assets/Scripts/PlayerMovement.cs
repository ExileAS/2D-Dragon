using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private Animator anim;
    [SerializeField] private float speed;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    private float horizontalInput;
    private bool isRunning;
    private float wallJumpCD = 0;
    private bool pressedSpacekey;



    private void Awake() {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update() {
        horizontalInput = Input.GetAxis("Horizontal");
        isRunning = horizontalInput > 0.01F || horizontalInput < -0.01F;
        body.gravityScale = (IsTouchingWall() && !IsGrounded()) ? 0.1f : 2;
        pressedSpacekey = Input.GetKeyDown(KeyCode.Space);
        Debug.Log(isRunning);

        // respond to horizontal movement
        

        // invert player x-axis scale (1 or -1)
        if(horizontalInput > 0.01F) {
            transform.localScale = Vector3.one;
        } 
        if(horizontalInput < -0.01F) {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        ProvideAnimParams();

        // respond to JUMP input
        if(pressedSpacekey && IsGrounded()) {
            if(IsTouchingWall()) {
                WallJump();
            } else {
                Jump();
            }
        }

        if(wallJumpCD > 0.2) {
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
            if(IsTouchingWall() && !IsGrounded() && pressedSpacekey) {
                if(isRunning) {
                    WallJump();
                } else {
                    WallJump2();
                }
            } 
        } else {
                wallJumpCD += Time.deltaTime;
            }
    }

    private void Jump() {
        body.velocity = new Vector2(body.velocity.x, speed);
        anim.SetTrigger("jump trigger");
    }

    private void OnCollisionEnter2D(Collision2D other) {
    }

    private bool IsGrounded() {
        RaycastHit2D raycastBox = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastBox.collider != null;
    }

    private bool IsTouchingWall() {
        RaycastHit2D raycastBox = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x,0), 0.1f, wallLayer);
        return raycastBox.collider != null;
    }

    private void WallJump() {
        body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x), 6);
        wallJumpCD = 0;
        anim.SetTrigger("jump trigger");
    }

    private void WallJump2() {
        body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 10, 0);
        transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        wallJumpCD = 0;
        anim.SetTrigger("jump trigger");
    }

    private void ProvideAnimParams() {
        // supply animator with "is running" value
        anim.SetBool("is running", isRunning);

        // supply animator with "is grounded" value
        anim.SetBool("is grounded", IsGrounded());
    }
}
