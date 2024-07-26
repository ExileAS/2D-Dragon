using UnityEngine;

public class Damage : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private float IFrameTime;

    protected void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")) {
            other.GetComponent<Health>().TakeDamage(damage, IFrameTime);
        }
    }
}
