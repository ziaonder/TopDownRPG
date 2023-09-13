using System;
using System.Collections;
using UnityEngine;

public class KamikazzyController : PatrolManager
{
    private float followTime = 5f;
    private float attackRange = 2f;
    private int kamikazzyDamage = 10;
    [SerializeField] private GameObject AOEPrefab;
    public static event Action<int> OnKamikazzyDamage;
    private bool hasFoundTarget;

    private void Awake()
    {
        health = 10;
        spriteRenderer = GetComponent<SpriteRenderer>();
        patrolPosition = transform.position;
        detectedSpriteObject = transform.Find("SpritePlayerDetected").gameObject;
        target = GameObject.Find("Player").transform;
    }

    private void Start()
    {
        detectionRadius = 5f;
        velocity = 2f;
    }

    private void Update()
    {
        if (!isPlayerDetected) DetectPlayer();
        Patrol(isPatrolCallable);

        
    }

    protected override IEnumerator LockOnTargetAndAttack()
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
                    OnKamikazzyDamage?.Invoke(kamikazzyDamage);
                }
            }

            yield return new WaitForSeconds(1f);
            Destroy(AOE);
            Destroy(gameObject);
        }

        yield return null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position, attackRange);
    }
}
