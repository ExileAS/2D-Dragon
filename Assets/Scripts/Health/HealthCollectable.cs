using UnityEngine;

public class HealthCollectable : MonoBehaviour
{
    [SerializeField] private float healthValue;
    public string id;

    [ContextMenu("generate ID")]
    private void GenerateGuid() {
        id = System.Guid.NewGuid().ToString();
    }
    [HideInInspector] public bool collected;

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")) {
            other.GetComponent<Health>().Heal(healthValue);
            gameObject.SetActive(false);
            collected = true;
        }
    }
}
