using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource gameMusic;

    private void Start()
    {
        gameMusic.Play();
    }
}
