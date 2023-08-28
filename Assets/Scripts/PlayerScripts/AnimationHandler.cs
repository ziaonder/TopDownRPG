using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    private Animator animator;
    private float currentHealth;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        currentHealth = PlayerHealth.GetHealth();
    }

    private void Update()
    {
        if (currentHealth > PlayerHealth.GetHealth())
        {;
            currentHealth = PlayerHealth.GetHealth();
            animator.SetBool("isRunning", false);
            animator.SetTrigger("Hurt");
        }
    }
    private void OnEnable()
    {
        PlayerMovement.OnAnimationChange += ChangeAnimation;
    }

    private void OnDisable()
    {
        PlayerMovement.OnAnimationChange -= ChangeAnimation;
    }

    private void ChangeAnimation(string state, bool whatIs)
    {
        animator.SetBool(state, whatIs);
    }
}
