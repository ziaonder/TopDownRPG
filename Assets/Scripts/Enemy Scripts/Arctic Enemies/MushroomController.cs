using System.Collections;
using UnityEngine;

public class MushroomController : PatrolManager
{
    private GameObject AOE;
    private SpriteRenderer spriteAOE;
    [SerializeField] private GameObject projectilePrefab;
    private float maxProjectileVelocity = 3f;

    private void Awake()
    {
        health = 10;
        AOE = transform.Find("MushroomAOE").gameObject;
        spriteAOE = AOE.GetComponent<SpriteRenderer>();
        patrolPosition = transform.position;
        detectedSpriteObject = transform.Find("SpritePlayerDetected").gameObject;
        target = GameObject.Find("Player").transform;
    }

    void Start()
    {
        detectionRadius = 5f;
        velocity = 1f;
    }

    void Update()
    {
        Patrol(isPatrolCallable);
        if (!isPlayerDetected) DetectPlayer();

        if(Vector2.Distance(transform.position, target.position) < detectionRadius)
            transform.rotation = transform.position.x > target.position.x ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);

        if(state == State.PATROLLING)
        {
            transform.rotation = transform.position.x > patrolPosition.x ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
        }
    }

    protected override IEnumerator LockOnTargetAndAttack()
    {
        isPlayerDetected = true;
        float rate = Random.Range(0f, 1f);

        if (rate < 0.4f)
        {
            StartCoroutine(AttackSurroundings());
            yield return null;
        }
        else
        {
            yield return new WaitForSeconds(1);

            Vector3 targetPosition = new Vector3(target.position.x, target.position.y - 0.2f);

            if (Vector3.Distance(transform.position, targetPosition) < detectionRadius)
            {
                StartCoroutine(LaunchProjectile(targetPosition));
            }
        }

        isPlayerDetected = false;
    }

    private IEnumerator AttackSurroundings()
    {
        isAttacking = true;
        yield return new WaitForSeconds(1f);
        AOE.GetComponent<CircleCollider2D>().enabled = false;
        AOE.SetActive(true);
        spriteAOE.color = Color.white;
        yield return new WaitForSeconds(2f);
        spriteAOE.color = Color.yellow;
        AOE.GetComponent<CircleCollider2D>().enabled = true;
        yield return new WaitForSeconds(1f);
        AOE.SetActive(false);
        isAttacking = false;
    }

    private IEnumerator LaunchProjectile(Vector3 targetPosition) 
    {
        isAttacking = true;
        GameObject instantiatedAOE = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        GameObject instantiatedAOE2 = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Vector3 normalizedDirection = (targetPosition - transform.position).normalized;
        Vector3 mirrorTargetPosition = transform.position + -normalizedDirection * Vector3.Distance(transform.position, targetPosition);

        StartCoroutine(LaunchMirrorProjectile(instantiatedAOE2, mirrorTargetPosition));
        
        while (instantiatedAOE != null && Vector2.Distance(instantiatedAOE.transform.position, targetPosition) > 0.1f)
        {
            instantiatedAOE.transform.position = Vector3.MoveTowards(instantiatedAOE.transform.position, targetPosition, 
                maxProjectileVelocity * Time.deltaTime);

            yield return null;
        }

        isAttacking = false;
    }

    private IEnumerator LaunchMirrorProjectile(GameObject mirrorObject, Vector3 mirrorTargetPosition)
    {
        while (mirrorObject != null &&  Vector2.Distance(mirrorObject.transform.position, mirrorTargetPosition) > 0.1f)
        {
            mirrorObject.transform.position = Vector3.MoveTowards(mirrorObject.transform.position, mirrorTargetPosition,
                maxProjectileVelocity * Time.deltaTime);

            yield return null;
        }
    }
}
