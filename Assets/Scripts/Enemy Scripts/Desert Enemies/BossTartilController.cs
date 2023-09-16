using System;
using System.Collections;
using UnityEngine;

public class BossTartilController : PatrolManager
{
    // Tartil Properties
    [SerializeField] private GameObject projectile, projectileAOE;
    private float maximumProjectileHeight = 3f, gravity = -9.807f;
    private float floatingTime;

    // Kamikazzy Properties
    private float followTime = 5f;
    private float attackRange = 4f;
    private int kamikazzyDamage = 50;
    [SerializeField] private GameObject AOEPrefab;
    public static event Action<int> OnBossKamikazzyDamage;
    private bool hasFoundTarget;

    private void OnEnable()
    {
        OnEnableArrangements();
    }

    private void OnDisable()
    {
        OnDisableArrangements();
    }

    private void Awake()
    {
        health = 100;
        spriteRenderer = GetComponent<SpriteRenderer>();
        patrolPosition = transform.position;
        detectedSpriteObject = transform.Find("SpritePlayerDetected").gameObject;
        target = GameObject.Find("Player").transform;
    }
    void Start()
    {
        detectionRadius = 5f;
        velocity = 2f;
    }

    void Update()
    {
        Patrol(isPatrolCallable);
    }

    private void FixedUpdate()
    {
        if (!isPlayerDetected) DetectPlayer();
    }

    protected override IEnumerator LockOnTargetAndAttack()
    {
        if(health > 10) // Tartil Attack
        {
            isPlayerDetected = true;
            detectedSpriteObject.SetActive(true);
            yield return new WaitForSeconds(2);
            Vector3 targetPosition = new Vector3(target.position.x, target.position.y - 0.2f);
            if (Vector3.Distance(transform.position, targetPosition) < detectionRadius)
            {
                LaunchProjectile(targetPosition);
                isAttacking = true;
            }

            isPlayerDetected = false;
            detectedSpriteObject.SetActive(false);
        }
        else // Kamikazzy Attack
        {
            if (!hasFoundTarget)
            {
                hasFoundTarget = true;

                while (followTime > 0f)
                {
                    transform.position = Vector2.MoveTowards(transform.position, target.position, velocity * Time.deltaTime);
                    followTime -= Time.deltaTime;
                    if (Vector2.Distance(transform.position, target.position) < 1f)
                        break;

                    yield return null;
                }

                GameObject AOE = Instantiate(AOEPrefab, transform.position, Quaternion.identity);
                Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, attackRange);

                foreach (Collider2D hit in hitObjects)
                {
                    if (hit.gameObject.name == "Player")
                    {
                        OnBossKamikazzyDamage?.Invoke(kamikazzyDamage);
                        Debug.Log("exploded");
                    }
                }

                yield return new WaitForSeconds(1f);
                Destroy(AOE);
                Destroy(gameObject);
            }
            yield return null;
        }
    }

    private void LaunchProjectile(Vector3 targetPosition)
    {
        GameObject instantiatedProjectile = Instantiate(projectile, transform.position, Quaternion.identity);
        Rigidbody2D projectileRb = instantiatedProjectile.GetComponent<Rigidbody2D>();
        projectileRb.gravityScale = 1f;
        projectileRb.velocity = CalculateLaunchVelocity(targetPosition);
        StartCoroutine(ActivateAreaOfEffect(floatingTime, targetPosition));
        StartCoroutine(DisableGravityOnLanding(projectileRb));
    }

    private Vector2 CalculateLaunchVelocity(Vector3 targetPosition)
    {
        float velocityX = 0f, tryParseOut;
        Vector3 velocityY;

        do // This do - while condition ensures velocity values are returned as float values.
        {
            float displacementX = target.transform.position.x - transform.position.x;
            float displacementY = targetPosition.y - transform.position.y;
            if (displacementY <= 0.1f)
                maximumProjectileHeight = 1.5f;
            else
                maximumProjectileHeight = 3f;

            float time = Mathf.Sqrt(Mathf.Abs((-2 * maximumProjectileHeight) / gravity)) + Mathf.Sqrt(Mathf.Abs((2 * (displacementY - maximumProjectileHeight)) / gravity));

            velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * maximumProjectileHeight);
            velocityX = displacementX / time;
            floatingTime = time;
        } while (!float.TryParse(velocityX.ToString(), out tryParseOut));

        return new Vector2(velocityX, velocityY.y);
    }

    private IEnumerator ActivateAreaOfEffect(float floatingTime, Vector3 targetPosition)
    {
        GameObject AOE = Instantiate(projectileAOE, targetPosition, Quaternion.identity);

        yield return new WaitForSeconds(floatingTime);
        Destroy(AOE);
    }

    private IEnumerator DisableGravityOnLanding(Rigidbody2D rb)
    {
        yield return new WaitForSeconds(floatingTime);

        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero;
        isPlayerDetected = false;
        isAttacking = false;
        rb.gameObject.GetComponent<Transform>().localScale = Vector3.one * 2;
        rb.gameObject.GetComponent<TartilProjectile>().isProjectileLanded = true;
    }
}
