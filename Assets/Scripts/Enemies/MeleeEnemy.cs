using UnityEngine;

public class MeleeEnemy : EnemyAttack
{

    [Header("SFX")]
    [SerializeField] private AudioClip swordAttack;

    protected override void Update()
    {
        base.Update();
        if (isAttacking) SFXManager.Instance.PlaySound(swordAttack);
    }
    private void DamagePlayer() {
        if(PlayerInSight() && !Physics2D.GetIgnoreLayerCollision(10, 11)) {
            playerHealth.TakeDamage(damage, 2, transform.position.x);
        }
    }
}
