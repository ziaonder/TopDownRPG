using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoldUIScript : MonoBehaviour
{
    private TextMeshProUGUI textMesh;
    private bool isGoldSet = false;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (!isGoldSet)
        {
            textMesh.text = PlayerHealth.gold.ToString();
            isGoldSet = true;
        }
    }

    public void ChangeGoldUIText(int gold)
    {
        textMesh.text = gold.ToString();
    }
}
