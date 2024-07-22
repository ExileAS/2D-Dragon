using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth;
    public float currentHealth { get; private set; }
    private Animator anim;
    private bool dead;

    [Header("IFrames")]
    private bool isInvulnerable;
    [SerializeField] private float IFrameTimer;
    [SerializeField] private float IFrameTimeLimit = 2;

    private void Awake() {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
    }

    private void Update() {
        if(isInvulnerable) {
            IFrameTimer += Time.deltaTime;
        }

        if(isInvulnerable && IFrameTimer > IFrameTimeLimit) {
            isInvulnerable = false;
            IFrameTimer = 0;
            Physics2D.IgnoreLayerCollision(10, 11, false);
        }
    }

    public void TakeDamage(float damage) {
        if(isInvulnerable || dead) return;
        
        currentHealth = Mathf.Clamp(currentHealth-damage, 0, maxHealth);

        if(currentHealth > 0) {
            anim.SetTrigger("hurt");
            isInvulnerable = true;
            Physics2D.IgnoreLayerCollision(10, 11, true);
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
}
