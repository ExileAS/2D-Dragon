using UnityEngine;

public class Cloud : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float lifeTime;
    [HideInInspector] public float timer;
    void Update()
    {
        if(timer > lifeTime) {
            if(CloudSpawner.inactiveClouds[CloudSpawner.currIndex] == null) {
                CloudSpawner.inactiveClouds[CloudSpawner.currIndex] = this;
                CloudSpawner.currIndex++;
                timer = 0;
            }
        } else {
            transform.Translate(speed * Time.deltaTime, 0, 0);
            timer += Time.deltaTime;
        }
    }
}
