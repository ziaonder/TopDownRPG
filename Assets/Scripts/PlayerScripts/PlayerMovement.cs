using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Vector2 mousePosition;
    [SerializeField] private float speed;
    private bool isClicked;
    private Animator _animator;
    [SerializeField] private GameObject redBullet;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    private void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(1))
            Instantiate(redBullet, transform.position, Quaternion.identity);
    }
    private void Update()
    {
        if (transform.position == new Vector3(mousePosition.x, mousePosition.y, 0))
        {
            _animator.SetBool("RunningLeft", false);
            _animator.SetBool("RunningRight", false);
            _animator.SetBool("Idle", true);
        }
        
        if (Input.GetMouseButton(1))
        {
            RaycastHit raycastHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out raycastHit, 100f))
            {
                if (raycastHit.transform != null)
                {
                    Instantiate(redBullet, transform.position, Quaternion.identity);
                    Debug.Log("hit");
                }
            }
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isClicked = true;

            if(transform.position.x < Camera.main.ScreenToWorldPoint(Input.mousePosition).x)
            {
                _animator.SetBool("RunningRight", true);
                _animator.SetBool("Idle", false);
                _animator.SetBool("RunningLeft", false);
                //right walking
            }
            else if(transform.position.x > Camera.main.ScreenToWorldPoint(Input.mousePosition).x)
            {
                _animator.SetBool("RunningRight", false);
                _animator.SetBool("Idle",  false);
                _animator.SetBool("RunningLeft", true);
                //left walking
            }
        }

        if(isClicked)
            transform.position = Vector3.MoveTowards(transform.position, mousePosition, speed * Time.deltaTime);
    }
}
