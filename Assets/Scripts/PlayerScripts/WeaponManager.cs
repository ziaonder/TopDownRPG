using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private Image clubUI, axeUI, spearUI, pistolUI;
    private SpriteRenderer sRenderer;
    private float initialRotationMelee = 11, finalRotationMelee = -48f, lerpSpeed = 0.01f, attackSpeed = 8f, drawSpeed = 3f;
    private float idleRotationSpear = 67, attackRotationSpear = 0;
    private bool isGoingDownWards = true;
    public static bool isAttacking = false;
    private Transform attackPointTransform;
    private float attackRange = 1f;
    public static event Action<GameObject, int> OnEnemyHit;
    public AudioClip clubSound, swingSound, spearSound, axeSound, pistolSound;
    [SerializeField] private AudioSource weaponHitSoundSource, swingSoundSource;  
    [SerializeField] private Sprite clubSprite, axeSprite, spearSprite, pistolSprite;
    [SerializeField] private GameObject bulletPrefab;
    private enum HoldingWeapon { Club, Axe, Spear, Pistol };
    HoldingWeapon holdingWeapon = HoldingWeapon.Club;
    private int clubDamage = 3, axeDamage = 6, spearDamage = 3, pistolDamage = 3;
    private int weaponDamage = 3, bulletVelocity = 10;
    private Vector3 initialPosition, pistolInitialPosition, projectileDifference, mirrorProjectileDifference;
    private struct WeaponSpecs
    {
        public Sprite sprite;
        public int damage;
        public Quaternion initialRotation, finalRotation;
        public Vector3 initialPosition;

        public WeaponSpecs(Sprite sprite, int damage, Quaternion initialRotation, Quaternion finalRotation, Vector3 initialPosition)
        {
            this.sprite = sprite;
            this.damage = damage;
            this.initialRotation = initialRotation;
            this.finalRotation = finalRotation;
            this.initialPosition = initialPosition;
        }
    }
    WeaponSpecs club, axe, spear, pistol;

    private void Awake()
    {
        projectileDifference = new Vector3(0.79f, 0.46f, 0);
        mirrorProjectileDifference = new Vector3(-0.79f, 0.46f, 0);
        weaponHitSoundSource.clip = clubSound;
        initialPosition = new Vector3(0.47f, -0.45f, 0f);
        pistolInitialPosition = new Vector3(0.76f, -0.45f, 0f);
        club = new WeaponSpecs(clubSprite, clubDamage, Quaternion.Euler(0, 0, initialRotationMelee), Quaternion.Euler(0, 0, finalRotationMelee), initialPosition);
        axe = new WeaponSpecs(axeSprite, axeDamage, Quaternion.Euler(0, 0, initialRotationMelee), Quaternion.Euler(0, 0, finalRotationMelee), initialPosition);
        spear = new WeaponSpecs(spearSprite, spearDamage, Quaternion.Euler(0, 0, idleRotationSpear), Quaternion.Euler(0, 0, attackRotationSpear), initialPosition);
        pistol = new WeaponSpecs(pistolSprite, pistolDamage, Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, 12.5f), pistolInitialPosition);

        swingSoundSource.clip = swingSound;
        sRenderer = GetComponent<SpriteRenderer>();
        attackPointTransform = transform.Find("AttackPoint");
    }

    private void Start()
    {
        ChangeWeapon(holdingWeapon);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && !isAttacking)
        {
            ChangeWeapon(HoldingWeapon.Club);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && !isAttacking)
        {
            ChangeWeapon(HoldingWeapon.Axe);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) && !isAttacking)
        {
            ChangeWeapon(HoldingWeapon.Spear);
        }

        if(Input.GetKeyDown(KeyCode.Alpha4) && !isAttacking)
        {
            ChangeWeapon(HoldingWeapon.Pistol);
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isAttacking)
        {
            isAttacking = true;

            if(holdingWeapon == HoldingWeapon.Club || holdingWeapon == HoldingWeapon.Axe)
            {
                Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPointTransform.position, attackRange, LayerMask.GetMask("Enemy"));

                if (hitObjects.Length != 0)
                {
                    weaponHitSoundSource.Play();
                    foreach (Collider2D hitObject in hitObjects)
                    {
                        OnEnemyHit(hitObject.gameObject, weaponDamage);
                    }
                }
                else
                    swingSoundSource.Play();
            }
            else if(holdingWeapon == HoldingWeapon.Spear)
            {
                float weaponPositionDifference;
                Vector2 boxSize = new Vector2(2.56f, 0.36f);
                if (transform.parent.transform.rotation.eulerAngles.y == 0)
                    weaponPositionDifference = 2.09f;
                else
                    weaponPositionDifference = -2.09f;

                // 4.18 difference
                Collider2D[] hitColliders =
                    Physics2D.OverlapBoxAll(new Vector2(transform.position.x + weaponPositionDifference, transform.position.y), 
                    boxSize, 0f, LayerMask.GetMask("Enemy"));

                if(hitColliders.Length != 0)
                {
                    weaponHitSoundSource.Play();
                    foreach (Collider2D hit in hitColliders)
                    {
                        OnEnemyHit(hit.gameObject, weaponDamage);
                    }
                }else
                    swingSoundSource.Play();
            }
            else if(holdingWeapon == HoldingWeapon.Pistol)
            {
                if(transform.parent.transform.rotation.eulerAngles.y == 0)
                {
                    StartCoroutine(MoveBullet(bulletVelocity, projectileDifference));
                }else
                    StartCoroutine(MoveBullet(-bulletVelocity, mirrorProjectileDifference));
                
                weaponHitSoundSource.Play();
            }
        }

        if (isAttacking)
        {
            if (holdingWeapon == HoldingWeapon.Club || holdingWeapon == HoldingWeapon.Axe)
            {
                // localRotation is used to follow the rotation of the parent object.
                transform.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(initialRotationMelee, finalRotationMelee, lerpSpeed));
            }
            else if(holdingWeapon == HoldingWeapon.Spear)
            {
                transform.localRotation = spear.finalRotation;
                transform.localPosition = 
                    new Vector3(initialPosition.x + Mathf.Lerp(0, 1.5f, lerpSpeed), transform.localPosition.y);
            }
            else if(holdingWeapon == HoldingWeapon.Pistol)
            {
                transform.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(pistol.initialRotation.eulerAngles.z, 
                    pistol.finalRotation.eulerAngles.z, lerpSpeed));
            }
            
            if (lerpSpeed > 1f)
            {
                isGoingDownWards = false;
            }

            if (lerpSpeed < 0f)
            {
                isGoingDownWards = true;
                isAttacking = false;

                if(holdingWeapon == HoldingWeapon.Spear)
                {
                    transform.localRotation = spear.initialRotation;
                    transform.localPosition = initialPosition;
                }

                if(holdingWeapon == HoldingWeapon.Pistol)
                {
                    transform.localPosition = pistol.initialPosition;
                    transform.localRotation = pistol.initialRotation;
                }

                lerpSpeed = 0.01f;
            }

            if (isGoingDownWards)
                lerpSpeed += Time.deltaTime * attackSpeed;
            else
                lerpSpeed -= Time.deltaTime * drawSpeed;
        }
    }

    private IEnumerator MoveBullet(int velocity, Vector3 difference)
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position + difference, Quaternion.identity);
        bullet.GetComponent<Rigidbody2D>().velocity = Vector2.right * velocity;
        yield return new WaitForSeconds(3f);
        Destroy(bullet);
    }
    private void ChangeWeapon(HoldingWeapon weaponType)
    {
        clubUI.color = Color.white;
        axeUI.color = Color.white;
        spearUI.color = Color.white;
        pistolUI.color = Color.white;

        switch (weaponType)
        {
            case HoldingWeapon.Club:

                clubUI.color = Color.green;
                sRenderer.sprite = club.sprite;
                transform.localPosition = club.initialPosition;
                weaponHitSoundSource.clip = clubSound;
                weaponDamage = club.damage;
                holdingWeapon = HoldingWeapon.Club;
                transform.localRotation = club.initialRotation;
                break;
            case HoldingWeapon.Axe:
                axeUI.color = Color.green;
                sRenderer.sprite = axe.sprite;
                transform.localPosition = axe.initialPosition;
                weaponHitSoundSource.clip = axeSound;
                weaponDamage = axe.damage;
                holdingWeapon = HoldingWeapon.Axe;
                transform.localRotation = axe.initialRotation;
                break;
            case HoldingWeapon.Spear:
                spearUI.color = Color.green;
                sRenderer.sprite = spear.sprite;
                transform.localPosition = spear.initialPosition;
                weaponHitSoundSource.clip = spearSound;
                weaponDamage = spear.damage;
                holdingWeapon = HoldingWeapon.Spear;
                transform.localRotation = spear.initialRotation;
                break;
            case HoldingWeapon.Pistol:
                pistolUI.color = Color.green;
                sRenderer.sprite = pistol.sprite;
                transform.localPosition = pistol.initialPosition;
                weaponHitSoundSource.clip = pistolSound;
                weaponDamage = pistol.damage;
                holdingWeapon = HoldingWeapon.Pistol;
                transform.localRotation = pistol.initialRotation;
                break;
        }
    }
}
