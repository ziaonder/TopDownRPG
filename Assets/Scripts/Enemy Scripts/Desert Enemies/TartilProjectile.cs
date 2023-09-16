using System;
using UnityEngine;

public class TartilProjectile : MonoBehaviour
{
    private float lifeTime = 5f;
    private int damage = 2;
    private float lastAttackTime = 0f;
    public static event Action<int> OnTartilAttack;
    [HideInInspector] public bool isProjectileLanded = false;

    private void Update()
    {
        if (isProjectileLanded)
        {
            lifeTime = lifeTime - Time.deltaTime;
            if (lifeTime < 0)
                Destroy(gameObject);
        }            
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isProjectileLanded)
        {
            if (gameObject.transform.localScale == Vector3.one * 2)
                damage = 10;
            if (collision.gameObject.name == "Player" && lastAttackTime == 0f) // Attacks immediately when the player enters the area.
            {
                OnTartilAttack?.Invoke(damage);
                lastAttackTime = lifeTime;
            }
            else if (collision.gameObject.name == "Player" && lastAttackTime - lifeTime > 1f) // Attacks every one second if the player inside the area.
            {
                OnTartilAttack?.Invoke(damage);
                lastAttackTime = lifeTime;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        lastAttackTime = 0f;
    }
}
