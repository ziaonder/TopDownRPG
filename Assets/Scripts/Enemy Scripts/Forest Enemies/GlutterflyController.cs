using System.Collections;
using UnityEngine;

public class GlutterflyController : PatrolManager
{
    [SerializeField] private GameObject projectile;
    private Animator animator;

    private void Awake()
    {
        health = 10;
        animator = GetComponent<Animator>();
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
        if(!isPlayerDetected) DetectPlayer();
        Patrol(isPatrolCallable);

        if (state == State.PATROLLING)
            animator.SetBool("isFlying", true);
        else
            animator.SetBool("isFlying", false);
    }
    
    protected override IEnumerator LockOnTargetAndAttack()
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

    #region deprecated
    //protected override void Patrol(bool isCallable)
    //{
    //    if (isCallable && !isPlayerDetected)
    //    {
    //        if (state == State.REACHED)
    //        {
    //            patrolPosition = transform.position + new Vector3((int)Random.Range(-5f, 5f), (int)Random.Range(-5f, 5f));
    //            if (CurrentTerrainLocator.LocateTerrain(patrolPosition) == "Forest")
    //                state = State.PATROLLING;
    //        }

    //        if (Vector3.Distance(transform.position, patrolPosition) < 0.1f)
    //        {
    //            state = State.REACHED;
    //            StartCoroutine(DisableEnablePatrolling());
    //        }

    //        if (state == State.PATROLLING)
    //        {
    //            //Vector3 direction = (patrolPosition - transform.position).normalized;
    //            if (Vector2.Distance(transform.position, patrolPosition) > .05f)
    //            {
    //                transform.position = Vector2.MoveTowards(transform.position, patrolPosition, velocity * Time.deltaTime);
    //                if (transform.position.x > patrolPosition.x)
    //                    spriteRenderer.flipX = false;
    //                else
    //                    spriteRenderer.flipX = true;
    //            }
    //        }
    //    }
    //}

    //private IEnumerator DisableEnablePatrolling()
    //{
    //    isPatrolCallable = false;
    //    yield return new WaitForSeconds(Random.Range(1f, 3f));
    //    isPatrolCallable = true;
    //}

    #endregion
}
