using System.Collections;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float lifeTime;
    [SerializeField] private float fadeDuration;
    private float timer;
    private SpriteRenderer spriteRenderer;
    private bool started;
    private float startAlpha;
    private Transform t;
    private Cloud[] inactiveClouds;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startAlpha = spriteRenderer.color.a;
        t = transform;
        inactiveClouds = CloudSpawner.inactiveClouds;
    }

    void Update()
    {
        t.Translate(speed * Time.deltaTime, 0, 0);
        if(timer > lifeTime) {
            if(inactiveClouds[CloudSpawner.currIndex] == null && !started) {
                StartCoroutine(FadeOut());
            }
        } else {
            timer += Time.deltaTime;
        }
    }

     private IEnumerator FadeOut()
    {
        started = true;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, 0, elapsedTime / fadeDuration);
            Color newColor = spriteRenderer.color;
            newColor.a = newAlpha;
            spriteRenderer.color = newColor;
            yield return null;
        }

        Color finalColor = spriteRenderer.color;
        finalColor.a = 0;
        spriteRenderer.color = finalColor;

        inactiveClouds[CloudSpawner.currIndex] = this;
        CloudSpawner.currIndex++;
        timer = 0;
        started = false;
    }

    public void ResetAlpha() {
        Color color = spriteRenderer.color;
        color.a = startAlpha;
        spriteRenderer.color = color;
    }
}
