using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private AudioSource audioSource;
    private enum CurrentTerrain { FOREST, DESERT, ARCTIC };
    private Dictionary<CurrentTerrain, AudioClip> footstepDict;
    public static event Action<string, bool> OnAnimationChange;

    [SerializeField] private AudioClip[] footstepClips = new AudioClip[3];
    private Vector2 mousePosition;
    private Camera cam;
    [SerializeField] float velocity;
    private Rigidbody2D _rigidbody;
    private bool isMovementRestricted;
    
    CurrentTerrain currentTerrain = CurrentTerrain.FOREST;
    private enum Target { LOCATED, REACHED, ENEMY }
    Target target;

    //public int GetHealth() { return GetComponent<PlayerHealth>().GetHealth(); }
    public void SetTargetToReached() { target = Target.REACHED; }
    private void OnEnable()
    {
        BoobyController.BoobyHitDamage += RestrictMovementIfDamaged;
    }
    private void OnDisable()
    {
        BoobyController.BoobyHitDamage -= RestrictMovementIfDamaged;
    }
    private void Awake(){
        footstepDict = new Dictionary<CurrentTerrain, AudioClip>()
        {
           { CurrentTerrain.FOREST, footstepClips[0] },
           { CurrentTerrain.DESERT, footstepClips[1] },
           { CurrentTerrain.ARCTIC, footstepClips[2] }
        };
        audioSource = GetComponent<AudioSource>();
        target = Target.REACHED;
        cam = Camera.main;
        mousePosition = transform.position;
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void RestrictMovementIfDamaged(int thisValueWillNotBeUsed)
    {
        target = Target.REACHED;
        isMovementRestricted = true;
        StartCoroutine(DisableMovementRestrictionInAMoment());
    }

    private IEnumerator DisableMovementRestrictionInAMoment()
    {
        yield return new WaitForSeconds(.2f);
        isMovementRestricted = false;
    }

    private void Update()
    {
        _rigidbody.AddForce(Vector2.zero);
        FootstepsManager(target);
        if(Input.GetMouseButton(1) && !isMovementRestricted)
        {
            if (Vector3.Distance(transform.position, mousePosition) > 0.2f)
            {
                target = Target.LOCATED;
                OnAnimationChange?.Invoke("isRunning", true);
                OnAnimationChange?.Invoke("isIdle", false);
            }
            mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        SetFaceDirection();

        if (Vector3.Distance(transform.position, mousePosition) < 0.2f || Input.GetKeyDown(KeyCode.W))
        {
            target = Target.REACHED;
            OnAnimationChange?.Invoke("isIdle", true);
            OnAnimationChange?.Invoke("isRunning", false);
        }
    }

    private void FixedUpdate()
    {
        if (target == Target.LOCATED)
        {
            Vector2 moveDirection = (mousePosition - _rigidbody.position).normalized;
            _rigidbody.MovePosition(_rigidbody.position + moveDirection * Time.fixedDeltaTime * velocity);
        }
    }

    private void FootstepsManager(Target target)
    {
        if (target == Target.LOCATED)
        {
            if(audioSource.clip != footstepDict[currentTerrain])
                audioSource.clip = footstepDict[currentTerrain];
            
            if(!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }
    }

    private void SetFaceDirection()
    {
        if (mousePosition.x > transform.position.x && !WeaponManager.isAttacking)
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        else if (!WeaponManager.isAttacking)
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name.Contains("Tilemap"))
        {
            switch (CurrentTerrainLocator.LocateTerrain(new Vector2(transform.position.x, transform.position.y)))
            {
                case "Forest":
                    currentTerrain = CurrentTerrain.FOREST;
                    break;
                case "Desert":
                    currentTerrain = CurrentTerrain.DESERT;
                    break;
                case "Arctic":
                    currentTerrain = CurrentTerrain.ARCTIC;
                    break;
            }
        }
    }
}
