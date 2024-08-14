using UnityEngine;

public class RangedEnemyHealth : EnemyHealth
{
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        if(dead) {
            GetComponent<RangedEnemy>().enabled = false;
            GetComponent<CapsuleCollider2D>().enabled = false;
        }
    }
}
