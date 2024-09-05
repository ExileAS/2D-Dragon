using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private Rigidbody2D body;
    private Animator anim;
    private CapsuleCollider2D capsuleCollider;

    [Header("Multipliers")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    [SerializeField] private float jumpBrakeMultiplier;

    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    [Header("SFX")]
    [SerializeField] private AudioClip jumpAudio;

    [Header("Coyote && Double Jump")]
    [SerializeField] private float coyoteJumpDuration;
    [SerializeField] private int maxJumpCount;

    private int jumpCount;
    private float coyoteJumpTimer;
    private float horizontalInput;
    private bool isRunning;
    private float wallJumpCD = 0;
    private bool pressedSpacekey;
    [HideInInspector] public bool dead;


    private void Awake() {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    private void Update() {
        if(dead) return;

        horizontalInput = Input.GetAxis("Horizontal");
        isRunning = horizontalInput > 0.1F || horizontalInput < -0.1F;
        body.gravityScale = (IsTouchingWall() && !IsGrounded()) ? 0.3f : 2;
        pressedSpacekey = Input.GetKeyDown(KeyCode.Space);

        if(IsGrounded()) {
            coyoteJumpTimer = coyoteJumpDuration;
            jumpCount = 0;
        }

        if(horizontalInput > 0.01F) {
            transform.localScale = Vector3.one;
        } 
        if(horizontalInput < -0.01F) {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        ProvideAnimParams();

        if(pressedSpacekey && (IsGrounded() || CanDoubleJump()) && !IsTouchingWall()) {
            Jump();
        }

        if(pressedSpacekey && !IsGrounded() && !IsTouchingWall() && CanCoyoteJump()) {
            CoyoteJump();
            coyoteJumpTimer = 0;
        }

        if(!IsGrounded() && coyoteJumpTimer > 0) {
            coyoteJumpTimer -= Time.deltaTime;
        }

        if(!IsGrounded() && Input.GetKeyUp(KeyCode.Space)) {
            body.velocity = new Vector2(body.velocity.x, body.velocity.y * jumpBrakeMultiplier);
        }

        if(wallJumpCD > 0.2) {
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
            if(IsTouchingWall()) {
                body.velocity = new Vector2(body.velocity.x, 0);
                if(pressedSpacekey) {
                    WallJump();
                }
            } 
        } else {
                wallJumpCD += Time.deltaTime;
            }
    }

    private void Jump() {
        body.velocity = new Vector2(body.velocity.x, jumpPower);
        anim.SetTrigger("jump trigger");
        SFXManager.Instance.PlaySound(jumpAudio);
        jumpCount++;
    }

    private void CoyoteJump() {
        body.velocity = new Vector2(body.velocity.x, jumpPower * 0.85f);
        anim.SetTrigger("jump trigger");
        SFXManager.Instance.PlaySound(jumpAudio);
        jumpCount++;
    }

    // private void OnCollisionEnter2D(Collision2D other) {
    //     if(other.gameObject.tag == "wallTest") return;
    //     Debug.Log(other.gameObject.name);
    // }

    private bool IsGrounded() {
        RaycastHit2D raycastCapsule = Physics2D.CapsuleCast(capsuleCollider.bounds.center, capsuleCollider.bounds.size, CapsuleDirection2D.Vertical, 0, Vector2.down, 0.1f, groundLayer);
        return raycastCapsule.collider != null && body.velocity.y == 0;
    }

    private bool IsTouchingWall() {
        RaycastHit2D raycastCapsule = Physics2D.CapsuleCast(capsuleCollider.bounds.center,capsuleCollider.bounds.size,CapsuleDirection2D.Horizontal,0,new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastCapsule.collider != null;
    }

    private void WallJumpVertical() {
        body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, 6);
        wallJumpCD = 0;
        anim.SetTrigger("jump trigger");
    }

    private void WallJumpHorizontal() {
        body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 10, 0);
        transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        wallJumpCD = 0;
        anim.SetTrigger("jump trigger");
    }

    private void WallJump() {
        if(isRunning) {
            WallJumpVertical();
        } else {
            WallJumpHorizontal();
        }
    }

    private void ProvideAnimParams() {
        anim.SetBool("is running", isRunning);
        anim.SetBool("is grounded", IsGrounded());
    }

    public bool CanAttack() {
        return IsGrounded() && !isRunning;
    }

    private bool CanDoubleJump() {
        return jumpCount < maxJumpCount;
    }

    private bool CanCoyoteJump() {
        return coyoteJumpTimer > 0 && body.velocity.y < 0;
    }

}
