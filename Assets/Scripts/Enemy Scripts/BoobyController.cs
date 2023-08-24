using System.Collections;
using UnityEngine;

public class BoobyController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Transform target;
    
    [SerializeField] private float h;
    private float gravity = -9.807f;
    private float floatingTime;
    private readonly float detectionRadius = 3f;
    private bool isPlayerDetected;
    private AudioSource hitSound;

    private void Awake()
    {
        target = GameObject.Find("Player").transform;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        hitSound = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            StartCoroutine(ResetPlayerVelocityAfterOneSecond(collision.gameObject));
            hitSound.Play();
        }    
    }

    private IEnumerator ResetPlayerVelocityAfterOneSecond(GameObject obj)
    {
        yield return new WaitForSeconds(0.2f);
        obj.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    private void Update()
    {
        DetectPlayer();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
    }
    private void DetectPlayer()
    {
        if(Vector3.Distance(transform.position, target.position) < detectionRadius && !isPlayerDetected)
        {
            StartCoroutine(LockOnTarget());
        }
    }
    private void Launch()
    {
        rb.gravityScale = 1f;
        rb.velocity = CalculateLaunchVelocity();
        StartCoroutine(DisableGravityOnLanding(rb));
    }
    private Vector2 CalculateLaunchVelocity()
    {
        float displacementX = target.transform.position.x - transform.position.x;
        float displacementY = target.transform.position.y - transform.position.y;
        if (displacementY <= 0.1f)
            h = 1.5f;
        else
            h = 3f;

        float time = Mathf.Sqrt((-2 * h) / gravity) + Mathf.Sqrt((2 * (displacementY - h)) / gravity);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * h);
        float velocityX = displacementX / time;
        floatingTime = time;
        return new Vector2(velocityX, velocityY.y);
    }

    private IEnumerator DisableGravityOnLanding(Rigidbody2D rb)
    {
        yield return new WaitForSeconds(floatingTime);
        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero;
        isPlayerDetected = false;
    }

    private IEnumerator LockOnTarget()
    {
        isPlayerDetected = true;
        yield return new WaitForSeconds(2);
        if (Vector3.Distance(transform.position, target.position) < detectionRadius)
        {
            Launch();
        }
        else
            isPlayerDetected = false;
    }
}