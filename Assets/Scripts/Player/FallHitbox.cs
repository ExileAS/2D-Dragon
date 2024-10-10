using UnityEngine;

public class FallHitbox : MonoBehaviour
{
    [SerializeField] private float enemyIFrames;
    [SerializeField] private int damage;
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.layer == PhysicsLayers.enemy) {
            other.GetComponent<EnemyHealth>().TakeDamage(damage, enemyIFrames);
        }
    }
}
