using UnityEngine;

public class PlungeMinDistance : MonoBehaviour
{
    [HideInInspector] public static bool touchingGround;

    private void OnTriggerExit2D(Collider2D other) {
        touchingGround = false;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        touchingGround = true;
    }
}
