using System;
using UnityEngine;
using TMPro;


public class Vidas : MonoBehaviour
{
    public static Vidas Instance { get; private set; }
    public int vidas = 3;

    public TMP_Text vidasText;

    private void Awake()
    {
        Instance = this;
    }


    public void ActualizarUI()
    {
        vidas--;
        Debug.Log("Vidas restantes: " + vidas);
        if (vidasText != null)
            vidasText.text = "Vidas: " + vidas;
    }
}