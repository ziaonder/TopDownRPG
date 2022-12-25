using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraFollowing : MonoBehaviour
{
    [SerializeField] private GameObject player;

    void Update()
    {
        if(player != null)
            transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);        
    }
}
