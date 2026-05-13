using UnityEngine;

[System.Serializable]
public struct DatosOleada
{
    [Header("Enemigos de esta oleada")]
    public int cantidadEnemigos;
    public float velocidadEnemigos;
    public int vidaEnemigos;
    public float tiempoEntreSpawns;
}

[CreateAssetMenu(fileName = "Oleadas_Nivel", menuName = "Juego/Configuracion de Oleadas")]
public class LevelWavesSO : ScriptableObject
{
    [Header("Ajustes Globales")]
    public float tiempoEntreOleadas = 5f;

    [Header("Lista de Oleadas")]
    [Tooltip("Define aquí cada oleada una por una")]
    public DatosOleada[] oleadas;
}