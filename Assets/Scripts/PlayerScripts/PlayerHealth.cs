using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance;
    private int health = 100;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
    }

    private void OnEnable()
    {
        BoobyController.HitDamage += GetDamage;
    }

    private void OnDisable()
    {
        BoobyController.HitDamage -= GetDamage;
    }

    public void GetDamage(int damage)
    {
        health -= damage;
        Debug.Log(health);
    }
}
