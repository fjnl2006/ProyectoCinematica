using UnityEngine;

[CreateAssetMenu(fileName = "WaveConfig", menuName = "Waves/WaveConfig")]
public class WaveConfig : ScriptableObject
{
    public int cantidadEnemigos = 3;
    public float velocidadEnemigos = 3f;
    public int vidaEnemigos = 3;
    public float tiempoEntreSpawns = 0.5f;
    public float anguloDesviacion = 15f; // variacion de angulo para que no vyan en fila recta

}