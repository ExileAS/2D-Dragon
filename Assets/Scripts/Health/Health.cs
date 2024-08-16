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
    private float hitImpactTimer;
    private bool gotHit;
    [SerializeField] private float pushBackMultiplier;
    [SerializeField] private float lerpDownSpeed;
    private float pushBackDirection;
    private float pushBackMaxValue;

    private void Awake() {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
        body = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        if(gotHit) {
            HitPushBack();
        }
    }

    public void TakeDamage(float damage, float IFrameTime = 1.5f, float? hitterPositionX = null) {
        currentHealth = Mathf.Clamp(currentHealth-damage, 0, maxHealth);

        if(currentHealth > 0) {
            if(hitterPositionX != null) {
                CalcPushBackMax((float) hitterPositionX);
                gotHit = true;
            }
            anim.SetTrigger("hurt");
            StartCoroutine(IFrames.CreateIFrames(spriteRend, IFrameTime));
        } else {
            if(!dead) {
                dead = true;
                anim.SetTrigger("die");
                GetComponent<PlayerMovement>().enabled = false;
                GetComponent<PlayerAttack>().enabled = false;
            }
        }
    }

    public void Heal(float value) {
        currentHealth = Mathf.Clamp(currentHealth + value, 0, maxHealth);
    }

    private void HitPushBack() {
        float pushBack = Mathf.Lerp(pushBackMaxValue, 0, lerpDownSpeed * Time.deltaTime);

        hitImpactTimer += Time.deltaTime;
        body.velocity = new Vector2(body.velocity.x - pushBack, body.velocity.y);

        if(hitImpactTimer > pushBackDuration) {
            gotHit = false;
            hitImpactTimer = 0;
        }
    }

    private void CalcPushBackMax(float hitterPositionX) {
        pushBackDirection = Mathf.Sign(hitterPositionX - transform.position.x);
        pushBackMaxValue = pushBackDirection * pushBackMultiplier;
    }
}
