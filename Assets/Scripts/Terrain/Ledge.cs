using System.Collections;
using UnityEngine;

public class Ledge : MonoBehaviour
{
    [SerializeField] private float reachLedgeDuration;
    [SerializeField] private float fallDownLedgeDuration;
    private bool started;
    private PlayerMovement player;
    private PlayerAttack playerAttack;

    private void Awake() {
        Physics2D.IgnoreLayerCollision(PhysicsLayers.player, PhysicsLayers.ledge, false);
    }

    private void Update() {
        if(player == null) return;
        if(Input.GetKey(KeyCode.X) && Input.GetKey(KeyCode.S) && player.IsOnledge() && !started) {
            StartCoroutine(IgnoreCollision(player.gameObject.layer, fallDownLedgeDuration));
            playerAttack.SuperFall();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(!other.CompareTag("Player")) return;
        StartCoroutine(IgnoreCollision(other.gameObject.layer, reachLedgeDuration));
    }

    private void OnCollisionEnter2D(Collision2D other) {
        player = other.gameObject.GetComponent<PlayerMovement>();
        playerAttack = other.gameObject.GetComponent<PlayerAttack>();
    }

    private IEnumerator IgnoreCollision(int playerLayer, float duration) {
        started = true;
        Physics2D.IgnoreLayerCollision(playerLayer, gameObject.layer, true);
        yield return new WaitForSeconds(duration);
        Physics2D.IgnoreLayerCollision(playerLayer, gameObject.layer, false);
        started = false;
    }
}
