using System;
using UnityEngine;

public class CollectableManager : MonoBehaviour
{
    public static event Action<int> OnHealUp, OnGoldCollect;
    private int healAmount = 10, goldAmount = 10;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.name == "Player" && gameObject.name.Contains("HP Pot"))
            OnHealUp?.Invoke(healAmount);
        else if(collision.gameObject.name == "Player" && gameObject.name.Contains("Gold"))
            OnGoldCollect?.Invoke(goldAmount);

        if(collision.gameObject.name == "Player")
            Destroy(gameObject);
    }
}
