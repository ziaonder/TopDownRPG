using System;
using System.Collections;
using UnityEngine;

public class BoobyController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Transform target;

    [SerializeField] private GameObject areaOfEffectObject, spritePlayerDetected;
    [SerializeField] private float maximumHeight;
    private float gravity = -9.807f;
    private float floatingTime;
    private readonly float detectionRadius = 3f, attackRadius = 1f;
    private int attackDamage = 5;
    private bool isPlayerDetected;
    private AudioSource hitSound;
    public static event Action<int> HitDamage;

    private void Awake()
    {
        areaOfEffectObject.GetComponent<SpriteRenderer>().color = Color.cyan;
        target = GameObject.Find("Player").transform;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        hitSound = GetComponent<AudioSource>();
    }

    private void Update()
    {
        DetectPlayer();
    }

    private void DetectPlayer()
    {
        if (Vector3.Distance(transform.position, target.position) < detectionRadius && !isPlayerDetected)
        {
            if (transform.position.x > target.position.x)
                transform.GetComponent<SpriteRenderer>().flipX = false; // Faces left.
            else
                transform.GetComponent<SpriteRenderer>().flipX = true;  // Faces right.

            StartCoroutine(LockOnTarget()); 
        }

        if (isPlayerDetected)
            spritePlayerDetected.SetActive(true);
        else
            spritePlayerDetected.SetActive(false);
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
        {
            isPlayerDetected = false;
        }
    }
    private void Launch()
    {
        rb.gravityScale = 1f;
        rb.velocity = CalculateLaunchVelocity();
        StartCoroutine(ActivateAreaOfEffect(floatingTime));
        StartCoroutine(DisableGravityOnLanding(rb));
    }

    private Vector2 CalculateLaunchVelocity()
    {
        float displacementX = target.transform.position.x - transform.position.x;
        float displacementY = target.transform.position.y - transform.position.y;
        if (displacementY <= 0.1f)
            maximumHeight = 1.5f;
        else
            maximumHeight = 3f;

        float time = Mathf.Sqrt((-2 * maximumHeight) / gravity) + Mathf.Sqrt((2 * (displacementY - maximumHeight)) / gravity);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * maximumHeight);
        float velocityX = displacementX / time;
        floatingTime = time;
        return new Vector2(velocityX, velocityY.y);
    }

    private IEnumerator ActivateAreaOfEffect(float floatingTime)
    {
        GameObject AOE = Instantiate(areaOfEffectObject, target.position, Quaternion.identity);
        SpriteRenderer spriteRenderer = AOE.GetComponent<SpriteRenderer>();
        float timer = 0, colorRate;
        
        while (timer < floatingTime)
        {
            timer += Time.deltaTime;
            colorRate = timer;
            if (timer > 1)
                colorRate = 1;
            spriteRenderer.color = Color.Lerp(Color.cyan, Color.red, colorRate);
            yield return null;
        }
        Destroy(AOE);
    }

    private IEnumerator DisableGravityOnLanding(Rigidbody2D rb)
    {
        yield return new WaitForSeconds(floatingTime);
        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero;
        isPlayerDetected = false;
        AttackOnRange();
    }

    private void AttackOnRange()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRadius, LayerMask.GetMask("Player"));

        if(hit != null)
        {
            HitDamage?.Invoke(attackDamage);
            hitSound.Play();
        }
    }
}