using UnityEngine;

public class MeleeEnemy : EnemyPatrol
{
    [Header("Attacking")]
    [SerializeField] private float damage;
    [SerializeField] private float attackCD;
    [Header("Hit Box Values")]
    [SerializeField] private float heightOffset;
    [SerializeField] private float hitBoxDimensions;
    [SerializeField] private float hitBoxScaleX;
    [SerializeField] private float hitBoxScaleY;
    [SerializeField] private float hitBoxRange;

    [Header("Player")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Health playerHealth;

    private float attackTimer;
    private bool isAttacking;

    protected override void Awake() {
        base.Awake();
    }
    protected override void Update() {
        base.Update();
        anim.SetBool("attacking", isAttacking);
        canMove = !PlayerInSight();
        bool canAttack = CanAttack();

        if(canAttack && playerHealth.currentHealth > 0) isAttacking = true;
    }

    private bool PlayerInSight() {
        Vector3 range = Vector3.right * hitBoxRange * Mathf.Sign(transform.localScale.x);
        Vector2 hitBoxSize = new Vector2(hitBoxDimensions * hitBoxScaleX, hitBoxDimensions * hitBoxScaleY);
        

        RaycastHit2D hitBox = Physics2D.BoxCast(new Vector3(transform.position.x, transform.position.y-heightOffset) + range, hitBoxSize, 0,
        Vector2.zero, 0, playerLayer);

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

    private void DamagePlayer() {
        if(PlayerInSight()) {
            playerHealth.TakeDamage(damage);
        }
    }
}
