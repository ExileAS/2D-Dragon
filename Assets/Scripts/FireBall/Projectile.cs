using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    private BoxCollider2D boxCollider;
    private Animator anim;
    private bool hit;
    private float direction;
    private float lifeTime;
    [SerializeField] private float projectileLifeTime;

    private void Awake() {
        boxCollider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

    private void Update() {
        if(hit) return;

        transform.Translate(speed * Time.deltaTime * direction, 0, 0);

        lifeTime += Time.deltaTime;

        if(lifeTime > projectileLifeTime) {
            Deactivate();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")) return;
        hit = true;
        boxCollider.enabled = false;
        anim.SetTrigger("explode");
    }

    // Called by an animation event at the last frame of the explosion animation
    private void Deactivate() {
        gameObject.SetActive(false);
    }

    public void SetDirection(float _direction) {
        hit = false;
        lifeTime = 0;
        direction = _direction;
        gameObject.SetActive(true);
        boxCollider.enabled = true;
        transform.localScale = new Vector3(_direction * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }
}
