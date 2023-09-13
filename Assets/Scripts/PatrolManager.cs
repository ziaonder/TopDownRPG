using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Tilemaps;

public class PatrolManager : MonoBehaviour
{
    protected float detectionRadius, velocity;
    protected Transform target;
    protected bool isPlayerDetected, isAttacking;
    protected GameObject detectedSpriteObject;
    protected enum State { PATROLLING, REACHED, ENEMYTARGETED }
    protected State state = State.REACHED;
    protected Vector3 patrolPosition;
    protected bool isPatrolCallable = true;
    protected SpriteRenderer spriteRenderer;
    protected int health;
    private void OnEnable()
    {
        WeaponManager.OnEnemyHit += GetDamage;   
        PistolBullet.OnBulletHit += GetDamage;
    }

    private void OnDisable()
    {
        WeaponManager.OnEnemyHit -= GetDamage;
        PistolBullet.OnBulletHit -= GetDamage;
    }
    
    private void GetDamage(GameObject hitObject, int hitDamage)
    {
        if (hitObject == gameObject)
        {
            health -= hitDamage;
        }

        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void DetectPlayer()
    {
        Vector3 targetPosition = new Vector3(target.position.x, target.position.y - 0.2f); // This centers the pivot of the player.
        if (Vector3.Distance(transform.position, targetPosition) <= detectionRadius)
        {
            detectedSpriteObject.SetActive(true);
            if(gameObject.name != "Mushroom")
                spriteRenderer.flipX = transform.position.x > target.position.x ? false : true;

            if (!isPlayerDetected && !isAttacking)
            {
                state = State.ENEMYTARGETED;
                StartCoroutine(LockOnTargetAndAttack());
            }
        }
        else
        {
            isPlayerDetected = false;
            detectedSpriteObject.SetActive(false);

            if(state == State.ENEMYTARGETED && !isAttacking)
                state = State.REACHED;
        }
    }

    protected virtual IEnumerator LockOnTargetAndAttack()
    {
        yield return null;
    }

    protected virtual void Patrol(bool isCallable)
    {
        if (isCallable && !isPlayerDetected)
        {
            if (state == State.REACHED)
            {
                patrolPosition = transform.position + new Vector3((int)Random.Range(-5f, 5f), (int)Random.Range(-5f, 5f));
                if (CurrentTerrainLocator.LocateTerrain(patrolPosition) == GetMovableTerrain())
                    state = State.PATROLLING;
            }

            if (Vector3.Distance(transform.position, patrolPosition) < 0.2f)
            {
                state = State.REACHED;
                StartCoroutine(DisableEnablePatrolling());
            }

            if (state == State.PATROLLING)
            {
                if (Vector2.Distance(transform.position, patrolPosition) > .05f)
                {
                    transform.position = Vector2.MoveTowards(transform.position, patrolPosition, velocity * Time.deltaTime);
                    if(gameObject.name != "Mushroom")
                        spriteRenderer.flipX = transform.position.x > patrolPosition.x ? false : true;
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

    private string GetMovableTerrain()
    {
        if (gameObject.name == "Booby" || gameObject.name == "Glutterfly")
            return "Forest";
        if (gameObject.name == "SlidingThing" ||  gameObject.name == "Mushroom")
            return "Arctic";
        else
            return "Desert";
    }
}
