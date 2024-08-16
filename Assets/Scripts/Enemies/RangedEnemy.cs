using UnityEngine;

public class RangedEnemy : EnemyAttack
{
    [Header("Projectiles")]
    [SerializeField] private GameObject[] fireBalls = new GameObject[10];
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform player;

    [Header("Melee Attack")]
    [SerializeField] private float MeleeAttackRange;
    private bool canMeleeAttack;

    protected override void Update() {
        base.Update();
        canMeleeAttack = Mathf.Abs(transform.position.x - player.position.x) <= MeleeAttackRange;
        
        if(isAttacking) {
            if(canMeleeAttack) {
                anim.SetBool("melee attacking", true);
            } else {
                anim.SetBool("melee attacking", false);
                RangedAttack();
            }
        }
    }

    private void RangedAttack() {
        int nextInd = ObjectPool.NextInPool(fireBalls);
        if(nextInd == -1) return;

        GameObject nextFireBall = fireBalls[nextInd];

        nextFireBall.transform.position = firePoint.position;
        nextFireBall.GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));
    }

    private void DamagePlayer() {
        if(PlayerInSight() && !Physics2D.GetIgnoreLayerCollision(10, 11) && canMeleeAttack) {
            playerHealth.TakeDamage(damage, 2, transform.position.x);
        }
    }

    // private void OnDrawGizmos() {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawLine(new Vector3(transform.position.x, transform.position.y-0.7f),
    //     new Vector3(transform.position.x + (Mathf.Sign(transform.localScale.x) * MeleeAttackRange), transform.position.y-0.7f));
    // }
}
