using System.Collections;
using UnityEngine;

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
    public GameObject hpPot, gold;

    public int GetHealth() { return health; }
    public void SetHealth(int argumentHealth) { health = argumentHealth; }
    protected void OnEnableArrangements()
    {
        WeaponManager.OnEnemyHit += GetDamage;
        PistolBullet.OnBulletHit += GetDamage;
    }

    protected void OnDisableArrangements()
    {
        WeaponManager.OnEnemyHit -= GetDamage;
        PistolBullet.OnBulletHit -= GetDamage;
    }
    
    public void SetStateToReached()
    {
        state = State.REACHED;
    }
    private void GetDamage(GameObject hitObject, int hitDamage)
    {
        if (hitObject == gameObject)
        {
            health -= hitDamage;
        }

        if(health <= 0)
        {
            bool isBoss = false;
            int chance = Random.Range(0, 100);
            if (gameObject.name.Contains("BossBooby"))
                isBoss = true;

            if (isBoss)
            {
                Vector3 position = new Vector3(transform.position.x + 0.5f, transform.position.y);
                Instantiate(gold, transform.position, Quaternion.identity);
                Instantiate(gold, transform.position, Quaternion.identity);
                Instantiate(gold, transform.position, Quaternion.identity);
                Instantiate(gold, transform.position, Quaternion.identity);
                Instantiate(hpPot, position, Quaternion.identity);
                Instantiate(hpPot, position, Quaternion.identity);
            }

            if (chance <= 30)
            {
                Instantiate(gold, transform.position, Quaternion.identity);

                if(chance <= 10)
                {
                    Vector3 position = new Vector3(transform.position.x + 0.5f, transform.position.y);
                    Instantiate(hpPot, position, Quaternion.identity);
                }
            }

            if (gameObject.name.Contains("BossBooby"))
                Destroy(gameObject.GetComponent<BoobyBossController>().objectAOE);

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
                //spriteRenderer.flipX = transform.position.x > target.position.x ? false : true;

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

    protected virtual IEnumerator LockOnTargetAndAttack() { yield return null; }

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
                    if(!gameObject.name.Contains("Mushroom") || gameObject.name == "BossMushroom")
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
        if (gameObject.name.Contains("Booby") || gameObject.name.Contains("Glutterfly") || gameObject.name.Contains("BossBooby"))
            return "Forest";
        if (gameObject.name.Contains("SlidingThing") ||  gameObject.name.Contains("Mushroom"))
            return "Arctic";
        else
            return "Desert";
    }
}
