using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private static float health = 100;
    private RectTransform hpScaler;

    private void Awake()
    {
        hpScaler = transform.Find("HP Scaler").GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        BoobyController.BoobyHitDamage += GetDamage;
        GlutterflyProjectile.GlutterflyHitDamage += GetDamage;
    }

    private void OnDisable()
    {
        BoobyController.BoobyHitDamage -= GetDamage;
        GlutterflyProjectile.GlutterflyHitDamage -= GetDamage;
    }

    public static float GetHealth()
    {
        return health;
    }

    public void GetDamage(int damage)
    {
        if(health > 0)
        {
            health -= damage;
            hpScaler.localScale = new Vector2(health / 100f, hpScaler.localScale.y);

        }

        if (health <= 0)
            hpScaler.localScale = new Vector2(0f, hpScaler.localScale.y);
    }
}
