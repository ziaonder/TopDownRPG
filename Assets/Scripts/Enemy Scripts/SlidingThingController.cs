using System.Collections;
using UnityEngine;

public class SlidingThingController : PatrolManager
{
    private float slideVelocity = 20f;
    private Rigidbody2D rb;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        detectedSpriteObject = transform.Find("SpritePlayerDetected").gameObject;
        patrolPosition = transform.position;
    }

    private void Start()
    {
        detectionRadius = 3f;
        velocity = 1f;
    }

    void Update()
    {
        Patrol(isPatrolCallable);
    }

    private void FixedUpdate()
    {
        if(!isPlayerDetected) DetectPlayer();
    }

    protected override IEnumerator LockOnTargetAndAttack() 
    {
        isPlayerDetected = true;
        state = State.ENEMYTARGETED;
        detectedSpriteObject.SetActive(true);
        yield return new WaitForSeconds(2);
        Vector3 targetPosition = new Vector3(target.position.x, target.position.y - 0.2f); // This centers the pivot of the player.
        if (Vector3.Distance(transform.position, targetPosition) <= detectionRadius)
        {
            StartCoroutine(SlideAttack(targetPosition));
        }
        else
        {
            isPlayerDetected = false;
            state = State.REACHED;
            detectedSpriteObject.SetActive(false);
        }

    }

    #region deprecated
    //private void DetectPlayer()
    //{
    //    if (Vector3.Distance(transform.position, target.position) <= detectionRadius && !isPlayerDetected)
    //    {
    //        StartCoroutine(LockOnTargetAndAttack());
    //    }
    //}

    //private void Patrol(bool isCallable)
    //{
    //    if (isCallable && !isPlayerDetected)
    //    {
    //        if (state == State.REACHED)
    //        {
    //            patrolPosition = transform.position + new Vector3((int)Random.Range(-5f, 5f), (int)Random.Range(-5f, 5f));
    //            if (CurrentTerrainLocator.LocateTerrain(patrolPosition) == "Forest")
    //                state = State.PATROLLING;
    //        }

    //        if (Vector3.Distance(transform.position, patrolPosition) < 0.2f)
    //        {
    //            state = State.REACHED;
    //            StartCoroutine(DisableEnablePatrolling());
    //        }

    //        if (state == State.PATROLLING)
    //        {
    //            Vector3 direction = (patrolPosition - transform.position).normalized;
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

    private IEnumerator SlideAttack(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        while(Vector3.Distance(targetPosition, transform.position) > 0.2f)
        {
            slideVelocity = Vector3.Distance(targetPosition, transform.position) < 1f ? 10f : 20f;
            rb.MovePosition(rb.transform.position + direction * Time.deltaTime * slideVelocity);
            yield return null;
        }

        if (Vector3.Distance(targetPosition, transform.position) < 0.2f)
            isPlayerDetected = false;
    }
}
