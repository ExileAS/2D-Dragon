using UnityEngine;

public class MeleeEnemy : EnemyAttack
{
    private void DamagePlayer() {
        if(PlayerInSight() && !Physics2D.GetIgnoreLayerCollision(10, 11)) {
            playerHealth.TakeDamage(damage, 2, transform.position.x);
        }
    }
}
