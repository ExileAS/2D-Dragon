using System.Collections;
using UnityEngine;

public static class IFrames {
    private static float numOfFlashes = 3;

    public static IEnumerator CreateIFrames(SpriteRenderer spriteRend, float IFrameTime) {
        Physics2D.IgnoreLayerCollision(10, 11, true);
        Physics2D.IgnoreLayerCollision(10, 13, true);
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
        Physics2D.IgnoreLayerCollision(10, 11, false);
        Physics2D.IgnoreLayerCollision(10, 13, false);
    }
} 
