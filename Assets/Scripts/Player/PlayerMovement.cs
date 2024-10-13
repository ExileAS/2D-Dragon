using System.Collections;
using UnityEngine;
using static AnimParamsPlayer;
using static PhysicsLayers;

public class PlayerMovement : MonoBehaviour, IDataPersistence
{
    [HideInInspector] public Rigidbody2D body;
    private Animator anim;
    private CapsuleCollider2D capsuleCollider;
    private SpriteRenderer spriteRenderer;

    [Header("Multipliers")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    [SerializeField] private float jumpBrakeMultiplier;

    [Header("Dash")]
    [SerializeField] private float initialDashForce;
    [SerializeField] private float dashForceIncrease;
    [SerializeField] private float dashCooldown;
    [SerializeField] private float dashDurationMin;
    [SerializeField] private float dashDurationMax;
    [SerializeField] private float dashBodyVelocityMax;

    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask ledgeLayer;
    [SerializeField] private LayerMask wallLayer;

    [Header("SFX")]
    [SerializeField] private AudioClip jumpAudio;

    [Header("Coyote && Double Jump")]
    [SerializeField] private float coyoteJumpDuration;
    [SerializeField] private int maxJumpCount;

    [Header("Cool Downs")]
    [SerializeField] private float WallJumpCD;
    [SerializeField] private float touchingWallDuration;

    [Header("Effects")]
    [SerializeField] private GameObject trail;

    private int jumpCount;
    private float coyoteJumpTimer;
    private float wallJumpTimer;
    private float touchingWallTimer;
    private bool isRunning;
    private bool touchedWall;
    private bool isDashing;
    private bool canDash = true;

    private struct PlayerInputs // for inputs affecting physics calcs.
    {
        public bool dash;
        public bool jump;
        public bool jumpCancel;
        public float horizontalInput; 
    }
    private PlayerInputs playerInputs;

    private void Awake() {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        transform.position = new Vector3(-6, 0, 0);
        trail.SetActive(false);
    }

    private void Update() {
        CollectInputs();
        LookLeftOrRight(playerInputs.horizontalInput);
        ProvideAnimParams();
        isRunning = playerInputs.horizontalInput > 0.1F || playerInputs.horizontalInput < -0.1F && !isDashing;
        float gravityScale = (IsTouchingWall() && !IsGrounded()) ? 0.3f : 2;
        body.gravityScale = isDashing ? 0 : gravityScale;

        if(IsGrounded()) {
            coyoteJumpTimer = coyoteJumpDuration;
            jumpCount = 0;
            touchedWall = false;
            touchingWallTimer = 0;
        }

        if(touchedWall && !IsTouchingWall()) {
            if(touchingWallTimer < touchingWallDuration) {
                touchingWallTimer += Time.deltaTime;
            }
            else {
                touchedWall = false;
                touchingWallTimer = 0;
            }
        }

        if(!IsGrounded() && coyoteJumpTimer > 0) {
            coyoteJumpTimer -= Time.deltaTime;
        }

        if(wallJumpTimer < WallJumpCD) {
            wallJumpTimer += Time.deltaTime;
        }
    }

    private void FixedUpdate() {
        if(playerInputs.dash && canDash) {
            StartCoroutine(Dash());
        }

        if(playerInputs.jump && CanJump()) {
            Jump();
        }

        if(playerInputs.jump && !IsGrounded() && !IsTouchingWall() && CanCoyoteJump()) {
            CoyoteJump();
            coyoteJumpTimer = 0;
        }

        if(playerInputs.jumpCancel && !IsGrounded()) {
            body.velocity = new Vector2(body.velocity.x, body.velocity.y * jumpBrakeMultiplier);
        }

        if(wallJumpTimer >= WallJumpCD) {
            if(!isDashing) body.velocity = new Vector2(playerInputs.horizontalInput * speed, body.velocity.y);
            if(IsTouchingWall() || touchedWall) {
                if(IsTouchingWall()) body.velocity = new Vector2(body.velocity.x, 0);
                if(playerInputs.jump) WallJump();
            }
        }
        ResetInputs();
    }

    private void CollectInputs() {
        if(Input.GetKeyDown(KeyCode.Space)) playerInputs.jump = true;
        if(Input.GetKey(KeyCode.LeftShift)) playerInputs.dash = true;
        if(Input.GetKeyUp(KeyCode.Space)) playerInputs.jumpCancel = true;
        playerInputs.horizontalInput = Input.GetAxisRaw("Horizontal");
    }

    private void ResetInputs() {
        playerInputs.dash = false;
        playerInputs.jump = false;
        playerInputs.jumpCancel = false;
        playerInputs.horizontalInput = 0;
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;
        isRunning = false;
        transform.position = new Vector2(transform.position.x, transform.position.y + 0.12f);
        float xForce = Mathf.Sign(transform.localScale.x) * initialDashForce;
        body.AddForce(new Vector2(xForce, 0), ForceMode2D.Impulse);
        float timeElapsed = 0;
        StartCoroutine(DashIFrames());
        while(timeElapsed < dashDurationMax) {
            if(Input.GetKey(KeyCode.LeftShift)) {
                body.AddForce(new Vector2(Mathf.Sign(transform.localScale.x) * dashForceIncrease, 0), ForceMode2D.Force);
            } 
            else if(timeElapsed >= dashDurationMin) break;
            if(timeElapsed > 0) body.velocity = new Vector2(Mathf.Clamp(body.velocity.x, -dashBodyVelocityMax, dashBodyVelocityMax), 0);
            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        body.velocity = new Vector2(0, body.velocity.y);
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private IEnumerator DashIFrames() {
        Color color = new(0.1f, 0.2f, 0.3f);
        trail.SetActive(true);
        anim.SetBool(dash, true);
        Physics2D.IgnoreLayerCollision(player, enemy, true);
        Physics2D.IgnoreLayerCollision(player, enemyProjectile, true);
        float timeElapsed = 0;
        while(isDashing) {
            spriteRenderer.color = color;
            timeElapsed += Time.deltaTime;
            color.r = Mathf.Lerp(color.r, 1, timeElapsed * timeElapsed / dashDurationMax);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();
        spriteRenderer.color = Color.white;
        trail.SetActive(false);
        anim.SetBool(dash, false);
        Physics2D.IgnoreLayerCollision(player, enemy, false);
        Physics2D.IgnoreLayerCollision(player, enemyProjectile, false);
    }

    private bool CanJump() {
        return (IsGrounded() || CanDoubleJump()) && !IsTouchingWall() && !touchedWall && !isDashing;
    }

    public bool CanAttack() {
        return !isDashing;
    }

    private void Jump() {
        body.velocity = new Vector2(body.velocity.x, jumpPower);
        anim.SetTrigger(jump);
        SFXManager.Instance.PlaySound(jumpAudio);
        jumpCount++;
    }

    private void CoyoteJump() {
        body.velocity = new Vector2(body.velocity.x, jumpPower * 0.85f);
        anim.SetTrigger(jump);
        SFXManager.Instance.PlaySound(jumpAudio);
        jumpCount++;
    }

    private bool IsGrounded() {
        RaycastHit2D raycastCapsule = Physics2D.CapsuleCast(capsuleCollider.bounds.center, capsuleCollider.bounds.size, 
        CapsuleDirection2D.Vertical, 0, Vector2.down, 0.1f, groundLayer);
        return (raycastCapsule.collider != null && body.velocity.y == 0) || IsOnledge();
    }

    public bool IsOnledge() {
        RaycastHit2D raycastCapsule = Physics2D.CapsuleCast(capsuleCollider.bounds.center, capsuleCollider.bounds.size, 
        CapsuleDirection2D.Vertical, 0, Vector2.down, 0.1f, ledgeLayer);
        return raycastCapsule.collider != null && body.velocity.y == 0 && !IsTouchingWall();
    }

    public bool IsInAir() {
        return !IsGrounded() && !IsTouchingWall();
    }

    private bool IsTouchingWall() {
        RaycastHit2D raycastHit = Physics2D.Raycast(
            capsuleCollider.bounds.center, 
            new Vector2(transform.localScale.x, 0).normalized,
            capsuleCollider.bounds.size.y / 2 + 0.1f,
            wallLayer
        );

        Debug.DrawLine(capsuleCollider.bounds.center,
        capsuleCollider.bounds.center + 
        new Vector3(transform.localScale.x, 0).normalized * (capsuleCollider.bounds.size.y / 2 + 0.1f));

        if (raycastHit.collider != null) {
            touchedWall = true;
        }

        return raycastHit.collider != null;
    }

    private void WallJumpVertical() {
        body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 4, 8);
        wallJumpTimer = 0;
        anim.SetTrigger(jump);
    }

    private void WallJumpHorizontal() {
        body.velocity = IsTouchingWall() ? new Vector2(-Mathf.Sign(transform.localScale.x) * 10, 0) : new Vector2(Mathf.Sign(transform.localScale.x) * 15, 0);
        transform.localScale = new Vector3((IsTouchingWall() ? -1 : 1) * Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        wallJumpTimer = 0;
        anim.SetTrigger(jump);
    }

    private void WallJump() {
        if(isRunning) {
            if(IsTouchingWall()) WallJumpVertical();
            else WallJumpHorizontal();
        } else {
            WallJumpHorizontal();
        }
    }

    private void ProvideAnimParams() {
        anim.SetBool(run, isRunning);
        anim.SetBool(grounded, IsGrounded());
        anim.SetBool(onWall, IsTouchingWall());
    }

    private bool CanDoubleJump() {
        return jumpCount >= 1 && jumpCount < maxJumpCount;
    }

    private bool CanCoyoteJump() {
        return coyoteJumpTimer > 0 && body.velocity.y < 0;
    }

    public void LookLeftOrRight(float horizontalInput) {
        if(isDashing) return;
        if(horizontalInput > 0.01F) {
            transform.localScale = Vector3.one;
        }
        if(horizontalInput < -0.01F) {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    public void LoadState(GameData data) {
        transform.position = data.latestCheckPoint;
    }

    public void SaveState(ref GameData data) {}
}
