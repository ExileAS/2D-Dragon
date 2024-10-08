using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDataPersistence
{
    [SerializeField] private float maxHealth;
    [SerializeField] private string id;
    public float currentHealth { get; private set; }
    protected bool dead;
    private Vector2 initialPosition;
    [SerializeField] private Animator anim;

    [Header("SFX")]
    [SerializeField] private AudioClip hurtAudio;
    [SerializeField] private AudioClip dieAudio;

    [ContextMenu("Generate id")]
    private void GenerateGuid() {
        id = System.Guid.NewGuid().ToString();
    }

    private void Awake() {
        currentHealth = maxHealth;
        initialPosition = transform.position;
    }

    public void TakeDamage(float damage, float? IFrameTime = null) {
        currentHealth = Mathf.Clamp(currentHealth-damage, 0, maxHealth);

        if(currentHealth > 0) {
            anim.SetTrigger("hurt");
            SFXManager.Instance.PlaySound(hurtAudio);
            if(IFrameTime != null) StartCoroutine(IFrames.CreateIFramesEnemy((float) IFrameTime));
        } else {
            if(!dead) {
                dead = true;
                SFXManager.Instance.PlaySound(dieAudio);
                KillEnemy();
            }
        }
    }

    private void KillEnemy() {
        anim.SetTrigger("die");
        currentHealth = 0;
        gameObject.layer = 20;
        GetComponent<EnemyPatrol>().enabled = false;
    }

    public void Respawn() {
        currentHealth = maxHealth;
        dead = false;
        anim.ResetTrigger("die");
        anim.Play("Idle");
        gameObject.layer = 11;
        transform.position = initialPosition;
        GetComponent<EnemyPatrol>().enabled = true;
    }

    public void SaveState(ref GameData data) {
        if(data.enemyState.ContainsKey(id)) {
            data.enemyState.Remove(id);
        }
        data.enemyState.Add(id, dead);
    }

    public void LoadState(GameData data) {
        data.enemyState.TryGetValue(id, out dead);
        if(dead) {
            KillEnemy();
        }
    }
}
