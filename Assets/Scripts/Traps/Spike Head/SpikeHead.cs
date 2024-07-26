using UnityEngine;

public class SpikeHead : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float distance;
    [SerializeField] private float attackCD;
    [SerializeField] private LayerMask playerLayer;
    private float attackTimer;
    private Vector3 destination;
    private bool attacking;
    private Vector2[] directions = new Vector2[4];
    private Vector3 initialPosition;
    private bool activated = true;

    private void Awake() {
        initialPosition = transform.position;
        CalcDirections();
    }

    private void Update() {
        if(!activated) return;

        if(attacking) {
            transform.Translate(speed * Time.deltaTime * destination);
        }
        else {
            attackTimer += Time.deltaTime;
            if(attackTimer > attackCD) {
                CheckForPlayer();
            }
        }
    }

    private void CheckForPlayer() {
        for (int i = 0; i < directions.Length; i++)
        {
            Debug.DrawRay(transform.position, directions[i], Color.red);
            RaycastHit2D rayHit = Physics2D.Raycast(transform.position, directions[i], distance, playerLayer);

            if(rayHit.collider != null && !attacking) {
                attacking = true;
                destination = directions[i];
                attackTimer = 0;
            }
        }
    }

    private void CalcDirections() {
        directions[0] = Vector2.down * distance;
        directions[1] = Vector2.right * distance;
        directions[2] = Vector2.up * distance;
        directions[3] = Vector2.left * distance;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        attacking = false;
    }

    public void ResetObject(bool _activated) {
        attacking = false;
        transform.position = initialPosition;
        attackTimer = 0;
        activated = _activated;
    }
} 
