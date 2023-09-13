using System;
using System.Collections;
using System.Collections.Generic;
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
            OnBulletHit?.Invoke(collision.gameObject, pistolDamage);
            audioSource.Play();
            Destroy(gameObject, 2f);
        }
    }
}
