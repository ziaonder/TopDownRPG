using System;
using System.Collections;
using UnityEngine;

public class PistolBullet : MonoBehaviour
{
    private int pistolDamage = 3;
    public static event Action<GameObject, int> OnBulletHit;
    [SerializeField] private AudioClip bulletHit;
    [SerializeField] private AudioSource audioSource;

    private void OnEnable()
    {
        audioSource.clip = bulletHit;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            float hitTime = Time.time;
            OnBulletHit?.Invoke(collision.gameObject, pistolDamage);
            audioSource.Play();
            StartCoroutine(DisableRenderer());
            GetComponent<CircleCollider2D>().enabled = false;
            Destroy(gameObject, 0.52f);
        }
    }

    private IEnumerator DisableRenderer()
    {
        yield return new WaitForSeconds(0.05f);
        GetComponent<SpriteRenderer>().enabled = false;
    }
}
