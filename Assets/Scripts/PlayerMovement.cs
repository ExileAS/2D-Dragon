using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private Animator anim;
    [SerializeField] private float speed;
    private bool isGrounded;
    private bool isRunning;



    private void Awake() {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update() {
        float horizontalInput = Input.GetAxis("Horizontal");
        isRunning = horizontalInput > 0.01F || horizontalInput < -0.01F;

        // respond to horizontal movement
        body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

        // invert player x-axis scale (1 or -1)
        if(horizontalInput > 0.01F) {
            transform.localScale = Vector3.one;
        } 
        if(horizontalInput < -0.01F) {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        ProvideAnimParams();

        // respond to JUMP input
        if(Input.GetKeyDown(KeyCode.Space) && isGrounded) Jump();
    }

    private void Jump() {
        body.velocity = new Vector2(body.velocity.x, speed);
        anim.SetTrigger("jump trigger");
        isGrounded = false;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.tag == "ground") {
            isGrounded = true;
        }
    }

    private void ProvideAnimParams() {
        // supply animator with "is running" value
        anim.SetBool("is running", isRunning);

        // supply animator with "is grounded" value
        anim.SetBool("is grounded", isGrounded);
    }
}
