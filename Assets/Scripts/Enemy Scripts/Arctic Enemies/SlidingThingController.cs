using System;
using System.Collections;
using UnityEngine;

public class SlidingThingController : PatrolManager
{
    private float slideVelocity = 3f;
    private int slideDamage = 6;
    private Rigidbody2D rb;
    public static event Action<int> OnSlideAttack;
    private bool isAbleToAttack = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        detectedSpriteObject = transform.Find("SpritePlayerDetected").gameObject;
        patrolPosition = transform.position;
        target = GameObject.Find("Player").transform;
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

    private IEnumerator SlideAttack(Vector3 targetPosition)
    {
        yield return new WaitForSeconds(0.5f);
        isAbleToAttack = true;

        while(Vector3.Distance(targetPosition, transform.position) > 0.2f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, slideVelocity * Time.deltaTime);
            yield return null;
        }

        if (Vector3.Distance(targetPosition, transform.position) < 0.2f)
        {
            isPlayerDetected = false;
            isAbleToAttack = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Player" && isAbleToAttack)
        {
            OnSlideAttack?.Invoke(slideDamage);
        }
    }
}
