using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    public float currentHealth { get; private set; }
    private Animator anim;
    private bool dead;

    private void Awake() {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
    }

    public void TakeDamage(float damage) {
        currentHealth = Mathf.Max(currentHealth - damage, 0);
        // currentHealth = Mathf.Clamp(currentHealth-damage, 0, maxHealth);

        if(currentHealth > 0) {
            // player hurt
            anim.SetTrigger("hurt");
            // create IFrames
        } else {
            // player die
            if(!dead) {
                dead = true;
                anim.SetTrigger("die");
                GetComponent<PlayerMovement>().enabled = false;
            }

        }
    }

    public void Heal(float value) {
        currentHealth = Mathf.Clamp(currentHealth + value, 0, maxHealth);
    }
}
