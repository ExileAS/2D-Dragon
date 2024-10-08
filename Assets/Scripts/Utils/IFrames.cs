using System.Collections;
using UnityEngine;
using static PhysicsLayers;

public static class IFrames {
    private static float numOfFlashes = 3;

    public static IEnumerator CreateIFrames(SpriteRenderer spriteRend, float IFrameTime) {
        Physics2D.IgnoreLayerCollision(player, enemy, true);
        Physics2D.IgnoreLayerCollision(player, enemyProjectile, true);
        for (int i = 0; i < numOfFlashes; i++)
        {
            spriteRend.color = new Color(1, 0.1f, 0.1f, 0.5f);
            yield return new WaitForSeconds(IFrameTime / (numOfFlashes * 3));
            spriteRend.color = Color.white;
            yield return new WaitForSeconds(IFrameTime / (numOfFlashes * 3));
            spriteRend.color = new Color(1, 0.1f, 0.1f, 0.5f);
            yield return new WaitForSeconds(IFrameTime / (numOfFlashes * 3));
        }
        spriteRend.color = Color.white;
        Physics2D.IgnoreLayerCollision(player, enemy, false);
        Physics2D.IgnoreLayerCollision(player, enemyProjectile, false);
    }

    public static IEnumerator CreateIFramesEnemy(float IFrameTime) {
        Debug.Log("ignoring");
        Physics2D.IgnoreLayerCollision(playerProjectile, enemy, true);
        yield return new WaitForSeconds(IFrameTime);
        Physics2D.IgnoreLayerCollision(playerProjectile, enemy, false);
    }
} 
