using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Cat : MonoBehaviour
{
    private float speed = 3;
    [SerializeField] private GameObject target;
    private Vector3 targetPos;

    private void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 3);
    }

    private void Start()
    {
        targetPos = transform.position;
    }
    private void DetectPlayer()
    {
        if (Physics2D.OverlapCircle(transform.position, 3, LayerMask.GetMask("Player")))
        {
            Debug.Log("found");
            targetPos = Physics2D.OverlapCircle(transform.position, 3, LayerMask.GetMask("Player")).transform.position;
        }     
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6)
            Destroy(collision.gameObject);
    }

    private void Update()
    {
        if(target != null)
            DetectPlayer();
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
    }
}
