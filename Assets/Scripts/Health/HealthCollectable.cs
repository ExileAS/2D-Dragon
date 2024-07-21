using UnityEngine;

public class HealthCollectable : MonoBehaviour
{
    [SerializeField] private float healthValue;

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")) {
            other.GetComponent<Health>().Heal(healthValue);
            gameObject.SetActive(false);
        }
    }
}
