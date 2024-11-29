using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static int health = 100;
    private static RectTransform hpScaler;
    public static int gold;
    public static event Action<int> OnHealthChange;
    [SerializeField] private AudioClip healUpClip, goldPickupClip;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject healthUI, goldUI, gameOverScreen;
    private bool isHealthSet = false;

    private void Awake()
    {
        hpScaler = transform.Find("HP Scaler").GetComponent<RectTransform>();
        gameOverScreen.SetActive(false);
    }

    private void Update()
    {
        if (!isHealthSet)
        {
            hpScaler.localScale = new Vector2(health / 100f, hpScaler.localScale.y);
            healthUI.GetComponent<HealthUI>().ChangeHealthTextUI(health);
            isHealthSet = true;
        }

        if(health <= 0)
        {
            Time.timeScale = 0f;
            gameOverScreen.SetActive(true);
            PlayerPrefs.SetString(PlayerPrefs.GetInt("SaveSlot").ToString(), " ");
        }
    }
    private void OnEnable()
    {
        BoobyController.BoobyHitDamage += GetDamage;
        GlutterflyProjectile.GlutterflyHitDamage += GetDamage;
        TartilProjectile.OnTartilAttack += GetDamage;
        MushroomAOE.OnMushroomDamage += GetDamage;
        SlidingThingController.OnSlideAttack += GetDamage;
        KamikazzyController.OnKamikazzyDamage += GetDamage;
        BossTartilController.OnBossKamikazzyDamage += GetDamage;
        CollectableManager.OnGoldCollect += AddGold;
        CollectableManager.OnHealUp += HealHP;
        BoobyBossController.BossBoobyHitDamage += GetDamage;
        BossMushroomController.OnBossSlideAttack += GetDamage;
    }

    private void OnDisable()
    {
        BoobyController.BoobyHitDamage -= GetDamage;
        GlutterflyProjectile.GlutterflyHitDamage -= GetDamage;
        TartilProjectile.OnTartilAttack -= GetDamage;
        MushroomAOE.OnMushroomDamage -= GetDamage;
        SlidingThingController.OnSlideAttack -= GetDamage;
        KamikazzyController.OnKamikazzyDamage -= GetDamage;
        BossTartilController.OnBossKamikazzyDamage -= GetDamage;
        CollectableManager.OnGoldCollect -= AddGold;
        CollectableManager.OnHealUp -= HealHP;
        BoobyBossController.BossBoobyHitDamage -= GetDamage;
        BossMushroomController.OnBossSlideAttack -= GetDamage;
    }

    private void AddGold(int amount)
    {
        gold += amount;
        goldUI.GetComponent<GoldUIScript>().ChangeGoldUIText(gold);
        audioSource.clip = goldPickupClip;

        audioSource.Play();
        Debug.Log(gold);
    }

    //public static void UpdateGoldUI()
    //{
    //    goldUI.GetComponent<GoldUIScript>().ChangeGoldUIText(gold);
    //}

    public static void GetDamage(int damage)
    {
        if(health > 0)
        {
            health -= damage;
            if(health < 0)
                health = 0;
            OnHealthChange?.Invoke(health);
            hpScaler.localScale = new Vector2(health / 100f, hpScaler.localScale.y);
        }
    }

    public void HealHP(int amount)
    {
        health += amount;
        audioSource.clip = healUpClip;
        audioSource.Play();
        if (health > 100)
            health = 100;
        Debug.Log(health);
        OnHealthChange?.Invoke(health);
        hpScaler.localScale = new Vector2(health / 100f, hpScaler.localScale.y);
        Debug.Log(health);
    }
}
