using UnityEngine;

public abstract class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    public float currentHealth { get; private set; }
    protected bool dead;
    [SerializeField] private Animator anim;

    private void Awake() {
        currentHealth = maxHealth;
    }
    public virtual void TakeDamage(float damage) { 
        currentHealth = Mathf.Clamp(currentHealth-damage, 0, maxHealth);

        if(currentHealth > 0) {
            anim.SetTrigger("hurt");
        } else {
            if(!dead) {
                dead = true;
                anim.SetTrigger("die");
            }
        }
    }

}
