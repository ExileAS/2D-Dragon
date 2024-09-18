using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth;
    private Rigidbody2D body;
    public float currentHealth { get; private set; }
    private Animator anim;
    private bool dead;
    private SpriteRenderer spriteRend;

    [Header("Hit Push Back")]
    [SerializeField] private float pushBackDuration;
    [SerializeField] private float pushBackMultiplier;
    [SerializeField] private float lerpDownSpeed;
    [SerializeField] private float pushAgainstHitSpeed;
    private float hitImpactTimer;
    private bool gotHit;

    [Header("SFX")]
    [SerializeField] private AudioClip hurtAudio;
    [SerializeField] private AudioClip dieAudio;
    [SerializeField] private AudioClip healAudio;

    [Header("Game Over")]
    [SerializeField] private UIManager uiManager;

    private float pushBackDirection;
    private float pushBack;
    private PlayerMovement playerMovement;

    private void Awake() {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
        body = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
        Physics2D.IgnoreLayerCollision(10, 11, false);
        Physics2D.IgnoreLayerCollision(10, 13, false);
    }

    private void Update() {
        if(gotHit) {
            playerMovement.enabled = false;
            HitPushBack();
        }
    }

    public void TakeDamage(float damage, float IFrameTime = 1.5f, float? hitterPositionX = null) {
        currentHealth = Mathf.Clamp(currentHealth-damage, 0, maxHealth);

        if(currentHealth > 0) {
            if(hitterPositionX != null) {
                CalcPushBackInitial((float) hitterPositionX);
                gotHit = true;
            }
            anim.SetTrigger("hurt");
            SFXManager.Instance.PlaySound(hurtAudio);
            StartCoroutine(IFrames.CreateIFrames(spriteRend, IFrameTime));
        } else {
            if(dead) return;

            dead = true;
            anim.SetTrigger("die");
            body.velocity = Vector2.zero;
            SFXManager.Instance.PlaySound(dieAudio);

            bool canRespawn = GetComponent<Respawn>().CanRespawn();
            if(canRespawn) {
                GetComponent<Respawn>().RespawnPlayer();
                dead = false;
            } else {
                GetComponent<PlayerMovement>().enabled = false;
                GetComponent<PlayerAttack>().enabled = false;
                uiManager.GameOverScreen();
            }
        }
    }

    public void Heal(float value) {
        currentHealth = Mathf.Clamp(currentHealth + value, 0, maxHealth);
        SFXManager.Instance.PlaySound(healAudio);
    }

    private void HitPushBack() {
        pushBack = Mathf.Lerp(pushBack, 0, lerpDownSpeed * Time.deltaTime);
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        playerMovement.LookLeftOrRight(horizontalInput);

        hitImpactTimer += Time.deltaTime;
        body.velocity = new Vector2(horizontalInput * pushAgainstHitSpeed - pushBack, body.velocity.y);

        if(hitImpactTimer > pushBackDuration) {
            gotHit = false;
            playerMovement.enabled = true;
            hitImpactTimer = 0;
        }
    }

    private void CalcPushBackInitial(float hitterPositionX) {
        pushBackDirection = Mathf.Sign(hitterPositionX - transform.position.x);
        pushBack = pushBackDirection * pushBackMultiplier;
    }
}
