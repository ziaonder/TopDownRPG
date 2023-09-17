using System;
using System.Collections;
using UnityEngine;

public class BoobyBossController : PatrolManager
{
    private Rigidbody2D rb;
    [SerializeField] private GameObject areaOfEffectObject;
    [SerializeField] private GameObject projectile;
    private float maximumHeight = 3;
    private float gravity = -9.807f;
    private float floatingTime;
    private readonly float attackRadius = 2f;
    private int attackDamage = 30;
    private bool isCollidable;
    private AudioSource hitSound;
    public static event Action<int> BossBoobyHitDamage;
    [HideInInspector] public GameObject objectAOE;
    private void Awake()
    {
        health = 100;
        patrolPosition = transform.position;
        areaOfEffectObject.GetComponent<SpriteRenderer>().color = Color.cyan;
        target = GameObject.Find("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        hitSound = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        detectedSpriteObject = transform.Find("SpritePlayerDetected").gameObject;
    }

    private void OnEnable()
    {
        OnEnableArrangements();
    }

    private void OnDisable()
    {
        OnDisableArrangements();
    }

    void Start()
    {
        velocity = 1.5f;
        rb.gravityScale = 0f;
        detectionRadius = 5f;
    }

    void Update()
    {
        Patrol(isPatrolCallable);
    }

    private void FixedUpdate()
    {
        if (!isPlayerDetected) DetectPlayer();
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.tag == "Player" && isCollidable)
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

    protected override IEnumerator LockOnTargetAndAttack()
    {
        isPlayerDetected = true;
        detectedSpriteObject.SetActive(true);
        yield return new WaitForSeconds(2);
        Vector3 targetPosition = new Vector3(target.position.x, target.position.y - 0.2f); // This centers the pivot of the player.
        if (Vector3.Distance(transform.position, targetPosition) < detectionRadius)
        {
            if(UnityEngine.Random.Range(1, 4) == 1)
            {
                Launch(targetPosition);
                isAttacking = true;
            }
            else
            {
                Vector3 position = new Vector3(transform.position.x, transform.position.y + 2);
                Instantiate(projectile, position, Quaternion.identity);
            }
        }

        isPlayerDetected = false;
        detectedSpriteObject.SetActive(false);
    }

    private void Launch(Vector3 targetPosition)
    {
        rb.gravityScale = 1f;
        rb.velocity = CalculateLaunchVelocity(targetPosition);
        StartCoroutine(ActivateAreaOfEffect(floatingTime, targetPosition));
        StartCoroutine(DisableGravityOnLanding(rb));
    }

    private Vector2 CalculateLaunchVelocity(Vector3 targetPosition)
    {
        float displacementX = target.transform.position.x - transform.position.x;
        float displacementY = targetPosition.y - transform.position.y;
        if (displacementY <= 0.1f)
            maximumHeight = 1.5f;
        else
            maximumHeight = 3f;

        float time = Mathf.Sqrt(Math.Abs((-2 * maximumHeight) / gravity)) + Mathf.Sqrt(Math.Abs((2 * (displacementY - maximumHeight)) / gravity));

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * maximumHeight);
        float velocityX = displacementX / time;
        floatingTime = time;
        return new Vector2(velocityX, velocityY.y);
    }

    private IEnumerator ActivateAreaOfEffect(float floatingTime, Vector3 targetPosition)
    {
        GameObject AOE = Instantiate(areaOfEffectObject, targetPosition, Quaternion.identity);
        objectAOE = AOE;
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
        isAttacking = false;
        AttackOnRange();
    }

    private IEnumerator ActivateCollisionForAMoment()
    {
        isCollidable = true;
        yield return new WaitForSeconds(0.2f);
        isCollidable = false;
    }

    private void AttackOnRange()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRadius, LayerMask.GetMask("Player"));

        if (hit != null)
        {
            BossBoobyHitDamage?.Invoke(attackDamage);
            hitSound.Play();
        }
    }
}
