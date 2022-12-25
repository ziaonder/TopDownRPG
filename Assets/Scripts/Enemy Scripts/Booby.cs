using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class Booby : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private Transform target;

    [SerializeField] private float h = 2f;
    [SerializeField] private float gravity;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Launch();
        }
        //Debug.Log(rb.velocity);
    }

    private void Launch()
    {
        rb.gravityScale = gravity;
        rb.velocity = CalculateLaunchVelocity();
    }
    private Vector2 CalculateLaunchVelocity()
    {
        float displacementX = target.transform.position.x - transform.position.x;
        float displacementY = target.transform.position.y - transform.position.y;
        float time = Mathf.Sqrt((-2 * h) / gravity) + Mathf.Sqrt((2 * (displacementY - h)) / gravity);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * h);
        float velocityX = displacementX / time;

        return new Vector2(velocityX, velocityY.y);
    }

}
