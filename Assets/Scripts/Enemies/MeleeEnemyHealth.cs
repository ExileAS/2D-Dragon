using UnityEngine;

public class MeleeEnemyHealth : EnemyHealth
{
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        if(dead) {
            GetComponent<MeleeEnemy>().enabled = false;
        }
    }

    public override void Respawn()
    {
        base.Respawn();
        GetComponent<MeleeEnemy>().enabled = true;
    }
}
