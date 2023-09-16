using UnityEngine;
using UnityEngine.UI;

public class NavigationArrow : MonoBehaviour
{
    public Transform playerTransform;
    public Transform forestBossTransform, desertBossTransform, arcticBossTransform;
    private Transform targetTransform;
    public Image arrowImage;
    public GameObject endScreen;

    void Update()
    {
        if(forestBossTransform != null)
            targetTransform = forestBossTransform;
        else if(desertBossTransform != null)
            targetTransform = desertBossTransform;
        else 
            targetTransform = arcticBossTransform;

        if(targetTransform != null)
        {
            Vector3 direction = (targetTransform.position - playerTransform.position);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            arrowImage.rectTransform.eulerAngles = new Vector3(0, 0, angle);
        }else
            endScreen.SetActive(true);
    }
}
