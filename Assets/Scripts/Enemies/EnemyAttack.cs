using UnityEngine;

public abstract class EnemyAttack : EnemyPatrol 
{
    [Header("Attacking")]
    [SerializeField] protected float damage;
    [SerializeField] private float attackCD;
    
    [Header("Hit Box Values")]
    [SerializeField] private float heightOffset;
    [SerializeField] private float hitBoxDimensions;
    [SerializeField] private float hitBoxScaleX;
    [SerializeField] private float hitBoxScaleY;
    [SerializeField] private float hitBoxRange;

    [Header("Player")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask doorLayer;
    protected Health playerHealth;
    protected bool isAttacking;

    protected override void Awake() {
        base.Awake();
        playerHealth = FindObjectOfType<Health>();
    }

    protected override void Update() {
        base.Update();
        anim.SetBool("attacking", isAttacking);
        canMove = !PlayerInSight();
        bool canAttack = CanAttack();

        if(canAttack && playerHealth.currentHealth > 0) isAttacking = true;
    }

    protected bool PlayerInSight() {
        Vector3 range = Vector3.right * hitBoxRange * Mathf.Sign(transform.localScale.x);
        Vector2 hitBoxSize = new Vector2(hitBoxDimensions * hitBoxScaleX, hitBoxDimensions * hitBoxScaleY);

        RaycastHit2D hitBox = Physics2D.BoxCast(new Vector3(transform.position.x, transform.position.y-heightOffset) + range, hitBoxSize, 0,
        Vector2.zero, 0, playerLayer);

        RaycastHit2D hitBoxDoor = Physics2D.BoxCast(new Vector3(transform.position.x, transform.position.y-heightOffset) + range, hitBoxSize, 0,
        Vector2.zero, 0, doorLayer);

        if(hitBox.collider != null && hitBoxDoor.collider != null) {
            float playerPositionX = Mathf.Abs(hitBox.collider.transform.position.x);
            float doorPositionX = Mathf.Abs(hitBoxDoor.collider.transform.position.x);
            return playerPositionX < doorPositionX;
        }
        return hitBox.collider != null;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(transform.position.x, transform.position.y-heightOffset) + Vector3.right * hitBoxRange * Mathf.Sign(transform.localScale.x), 
        new Vector2(hitBoxDimensions * hitBoxScaleX, hitBoxDimensions * hitBoxScaleY));
    }

    private bool CanAttack() {
        attackTimer += Time.deltaTime;

        if(attackTimer > attackCD && PlayerInSight()) {
            attackTimer = 0;
            return true;
        }
        else {
            isAttacking = false;
        }

        return false;
    }
}