using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float lifeTime;
    private float timer;
    

    private void Update() {
        transform.Translate(speed * Time.deltaTime, 0, 0);

        if(timer > lifeTime) {
            timer = 0;
            gameObject.SetActive(false);
        } else timer += Time.deltaTime;
    }

    public void ActivateArrow() {
        gameObject.SetActive(true);
        timer = 0;
    }
}
