using System;
using System.Collections;
using UnityEngine;

public class GlutterflyProjectile : MonoBehaviour
{
    private Vector3 playerPosition;
    private GameObject player;
    private Vector2 moveDirection;
    private Rigidbody2D rb;
    private float velocity = 3f, distanceThreshold = 0.1f;
    public static event Action<int> GlutterflyHitDamage;
    private int damage = 2;
    private AudioSource audioSource;
    [SerializeField] private AudioClip hitSound;

    private void OnEnable()
    {
        if (transform.localScale.x == 1.5f)
            damage = 10;
        else
            damage = 2;

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = hitSound;
        player = GameObject.Find("Player");
        playerPosition = player.transform.position;
        moveDirection = (playerPosition - transform.position).normalized;
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveDirection * Time.deltaTime * velocity);

        if (Vector2.Distance(transform.position, playerPosition) < distanceThreshold)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Player")
        {
            GlutterflyHitDamage?.Invoke(damage);
            audioSource.Play();
            GetComponent<SpriteRenderer>().enabled = false;
            StartCoroutine(DestroyObject());
        }
    }

    // This enumerator used for letting audioSource play before destroying the object.
    private IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
