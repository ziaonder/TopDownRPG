using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TartilController : PatrolManager
{
    [SerializeField] private GameObject projectile, projectileAOE;
    private float maximumProjectileHeight = 3f, gravity = -9.807f;
    private float floatingTime;

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
        health = 10;
        spriteRenderer = GetComponent<SpriteRenderer>();
        patrolPosition = transform.position;
        detectedSpriteObject = transform.Find("SpritePlayerDetected").gameObject;
        //rb = GetComponent<Rigidbody2D>();
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
        rb.gameObject.GetComponent<Transform>().localScale = Vector3.one;
        rb.gameObject.GetComponent<TartilProjectile>().isProjectileLanded = true;
    }
}
