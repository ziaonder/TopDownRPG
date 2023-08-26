using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

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
    private bool isPlayerDetected, isCollidable;
    private AudioSource hitSound;
    public static event Action<int> HitDamage;

    private void Awake()
    {
        areaOfEffectObject.GetComponent<SpriteRenderer>().color = Color.cyan;
        target = GameObject.Find("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        hitSound = GetComponent<AudioSource>();
    }

    private void Start()
    {
        rb.gravityScale = 0f;
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if(collider.tag == "Player" && isCollidable)
        {
            StartCoroutine(MovePlayer(collider));
        }
    }

    private IEnumerator MovePlayer(Collider2D collider)
    {
        collider.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        Vector2 closestPoint = collider.ClosestPoint(transform.position);

        if (closestPoint.x > transform.position.x)
            collider.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(-100f, 0f));
        else
            collider.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(100f, 0f));

        if (closestPoint.y > transform.position.y)
            collider.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 100f));
        else
            collider.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, -100f));

        yield return new WaitForSeconds(0.3f);

        collider.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
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
        StartCoroutine(ActivateCollisionForAMoment());

        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero;
        isPlayerDetected = false;
        AttackOnRange();
    }

    private IEnumerator ActivateCollisionForAMoment()
    {
        isCollidable = true;
        yield return new WaitForSeconds(0.2f);
        isCollidable =  false;
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