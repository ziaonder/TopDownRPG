using System;
using UnityEngine;

public class MushroomAOE : MonoBehaviour
{
    private int surroundingsDamage = 5, bossSurroundingsDamage = 10, projectileDamage = 2, bossProjectileDamage = 20;
    public static event Action<int> OnMushroomDamage;
    private GameObject player;
    private Vector3 playerPosition, mirrorPlayerPosition;
    private float distanceThreshold = 0.3f;

    private void OnEnable()
    {
        player = GameObject.Find("Player");
        playerPosition = player.transform.position;
        mirrorPlayerPosition = new Vector3(transform.position.x - (player.transform.position.x - transform.position.x),
            transform.position.y - (player.transform.position.y - transform.position.y));
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameObject.name.Contains("MushroomAOE")  && collision.gameObject.name == "Player")
        {
            if (gameObject.transform.localScale == Vector3.one)
                OnMushroomDamage?.Invoke(surroundingsDamage);
            else
                OnMushroomDamage?.Invoke(bossSurroundingsDamage);
        }
        else if(collision.gameObject.name == "Player")
        {
            if(gameObject.transform.localScale == Vector3.one * 0.3f)
                OnMushroomDamage?.Invoke(projectileDamage);
            else
                OnMushroomDamage?.Invoke(bossProjectileDamage);
            
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if(gameObject.name.Contains("MushroomProjectile(Clone)"))
        {
            if (Vector2.Distance(transform.position, playerPosition) < distanceThreshold)
                Destroy(gameObject);

            if (Vector2.Distance(transform.position, mirrorPlayerPosition) < distanceThreshold)
                Destroy(gameObject);
        }
    }
}
