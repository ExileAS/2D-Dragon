using UnityEngine;

public class MeleeEnemyHealth : EnemyHealth
{
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        if(dead) {
            GetComponent<MeleeEnemy>().enabled = false;
            GetComponent<CapsuleCollider2D>().enabled = false;
        }
    }
}
