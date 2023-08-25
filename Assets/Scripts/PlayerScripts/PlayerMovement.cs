using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private AudioSource audioSource;
    private enum CurrentTerrain { FOREST, DESERT, ARCTIC };
    private Dictionary<CurrentTerrain, AudioClip> footstepDict;

    [SerializeField] private AudioClip[] footstepClips = new AudioClip[3];
    private Vector2 mousePosition;
    private Camera cam;
    [SerializeField] float velocity;
    private Rigidbody2D _rigidbody;
    
    CurrentTerrain currentTerrain = CurrentTerrain.FOREST;
    private enum Target { LOCATED, REACHED, ENEMY }
    Target target;

    private void Awake(){
        footstepDict = new Dictionary<CurrentTerrain, AudioClip>()
        {
           { CurrentTerrain.FOREST, footstepClips[0] },
           //{ CurrentTerrain.DESERT, footstepClips[1] },
           { CurrentTerrain.ARCTIC, footstepClips[2] }
        };
        audioSource = GetComponent<AudioSource>();
        target = Target.REACHED;
        cam = Camera.main;
        mousePosition = transform.position;
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        FootstepsManager(target);
        if(Input.GetMouseButton(1))
        {
            if (Vector3.Distance(transform.position, mousePosition) > 0.05f)
                target = Target.LOCATED;

            mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);

            if (mousePosition.x > transform.position.x)
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            else
                transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }

        if(Vector3.Distance(transform.position, mousePosition) < 0.05f)
        {
            target = Target.REACHED;
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

    private void OnTriggerExit2D(Collider2D collision)
    {
        #region Find Current Terrain
        if (collision.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            switch (CurrentTerrainLocator.LocateTerrain(transform))
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
        #endregion
    }
}
