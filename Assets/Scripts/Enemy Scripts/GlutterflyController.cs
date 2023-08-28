using System.Collections;
using UnityEngine;

public class GlutterflyController : MonoBehaviour
{
    private float detectionRadius = 5f;
    [SerializeField] private Transform target;
    private bool isPlayerDetected;
    [SerializeField] private GameObject projectile, projectileRangeDedicator;
    private GameObject detectedSpriteObject;
    private enum GlutterflyState { PATROLLING, REACHED, ENEMYTARGETED }
    GlutterflyState state = GlutterflyState.REACHED;
    private Vector3 patrolPosition;
    private float velocity = 1f;
    private bool isPatrolCallable = true;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        patrolPosition = transform.position;
        detectedSpriteObject = transform.Find("SpritePlayerDetected").gameObject;
    }

    private void Update()
    {
        DetectPlayer();
        Patrol(isPatrolCallable);

        if (state == GlutterflyState.PATROLLING)
            animator.SetBool("isFlying", true);
        else
            animator.SetBool("isFlying", false);
    }
    private void DetectPlayer()
    {
        if(Vector3.Distance(transform.position, target.position) <= detectionRadius && !isPlayerDetected)
        {
            StartCoroutine(LockOnTargetAndAttack());
        }
    }
    
    private IEnumerator LockOnTargetAndAttack()
    {
        isPlayerDetected = true;
        detectedSpriteObject.SetActive(true);
        yield return new WaitForSeconds(2);
        
        if (Vector3.Distance(transform.position, target.position) <= detectionRadius)
        {
            Instantiate(projectile, transform.position, Quaternion.identity);
        }

        isPlayerDetected = false;
        detectedSpriteObject.SetActive(false);
    }

    private void Patrol(bool isCallable)
    {
        if (isCallable && !isPlayerDetected)
        {
            if (state == GlutterflyState.REACHED)
            {
                patrolPosition = transform.position + new Vector3((int)Random.Range(-5f, 5f), (int)Random.Range(-5f, 5f));
                if (CurrentTerrainLocator.LocateTerrain(patrolPosition) == "Forest")
                    state = GlutterflyState.PATROLLING;
            }

            if (Vector3.Distance(transform.position, patrolPosition) < 0.1f)
            {
                state = GlutterflyState.REACHED;
                StartCoroutine(DisableEnablePatrolling());
            }

            if (state == GlutterflyState.PATROLLING)
            {
                Vector3 direction = (patrolPosition - transform.position).normalized;
                if (Vector2.Distance(transform.position, patrolPosition) > .05f)
                {
                    transform.position = Vector2.MoveTowards(transform.position, patrolPosition, velocity * Time.deltaTime);
                    if (transform.position.x > patrolPosition.x)
                        spriteRenderer.flipX = false;
                    else
                        spriteRenderer.flipX = true;
                }
            }
        }
    }

    private IEnumerator DisableEnablePatrolling()
    {
        isPatrolCallable = false;
        yield return new WaitForSeconds(Random.Range(1f, 3f));
        isPatrolCallable = true;
    }
}
