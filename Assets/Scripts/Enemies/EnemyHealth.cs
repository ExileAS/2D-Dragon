using UnityEngine;

public abstract class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    public float currentHealth { get; private set; }
    protected bool dead;
    private Vector2 initialPosition;
    [SerializeField] private Animator anim;

    [Header("SFX")]
    [SerializeField] private AudioClip hurtAudio;
    [SerializeField] private AudioClip dieAudio;

    private void Awake() {
        currentHealth = maxHealth;
        initialPosition = transform.position;
    }

    public virtual void TakeDamage(float damage) { 
        currentHealth = Mathf.Clamp(currentHealth-damage, 0, maxHealth);

        if(currentHealth > 0) {
            anim.SetTrigger("hurt");
            SFXManager.Instance.PlaySound(hurtAudio);
        } else {
            if(!dead) {
                dead = true;
                anim.SetTrigger("die");
                SFXManager.Instance.PlaySound(dieAudio);
                gameObject.layer = 20;
            }
        }
    }

    public virtual void Respawn() {
        currentHealth = maxHealth;
        dead = false;
        anim.ResetTrigger("die");
        anim.Play("Idle");
        gameObject.layer = 11;
        transform.position = initialPosition;
    }
}
