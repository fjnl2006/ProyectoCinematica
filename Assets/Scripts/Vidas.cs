using System;
using UnityEngine;
using TMPro;


public class Vidas : MonoBehaviour
{
    public int vidas = 3;

    public TMP_Text vidasText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        vidas--;
        ActualizarUI();

        if (vidas <= 0)
        {
            Debug.Log("Game Over");
            // Aquí puedes cargar una escena de Game Over, etc.
        }

        
    }
    void ActualizarUI()
    {
        if (vidasText != null)
            vidasText.text = "Vidas: " + vidas;
    }
}