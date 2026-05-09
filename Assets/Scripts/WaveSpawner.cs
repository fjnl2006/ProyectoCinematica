using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public static WaveSpawner instance;

    [Header("Prefab y punto de spawn")]
    [SerializeField] private GameObject prefabEnemigo;
    [SerializeField] private Transform[] puntosSpawn;   // Varios posibles puntos de aparición
    [SerializeField] private Transform canonTransform;   // El Transform del cañón (objetivo de los enemigos)

    [Header("Configuración de oleadas")]
    [SerializeField] private int enemigosBase = 3;       // Enemigos en la oleada 1
    [SerializeField] private int extraPorOleada = 2;     // Enemigos extra por cada oleada
    [SerializeField] private float tiempoEntreOleadas = 5f;
    [SerializeField] private float tiempoEntreSpawns = 0.5f;

    [Header("Escalado de dificultad")]
    [SerializeField] private float velocidadBase = 3f;
    [SerializeField] private float velocidadExtraPorOleada = 0.3f;
    [SerializeField] private int vidaBase = 3;
    [SerializeField] private int vidaExtraPorOleada = 1;
    
    public int currentBulletDamage = 1;
    public float currentRadius = 3;
    
    // Estado interno
    private int oleadaActual = 0;
    private int enemigosVivos = 0;
    private bool esperando = false;

    // Evento opcional para UI
    public System.Action<int> onNuevaOleada;
    public System.Action onJuegoTerminado;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        StartCoroutine(RutinaOleadas());
    }

    IEnumerator RutinaOleadas()
    {
        while (true)
        {
            // Esperar a que no queden enemigos (excepto en la primera oleada)
            if (oleadaActual > 0)
            {
                esperando = true;
                yield return new WaitUntil(() => enemigosVivos <= 0);
                esperando = false;
                yield return new WaitForSeconds(tiempoEntreOleadas);
            }

            oleadaActual++;
            if (oleadaActual % 2 == 0)
            {
                currentBulletDamage++; 
            }

            if (oleadaActual == 6)
            {
                currentRadius += 0.5f;
            }
            int cantidadEnemigos = enemigosBase + (oleadaActual - 1) * extraPorOleada;
            float velEnemigos = velocidadBase + (oleadaActual - 1) * velocidadExtraPorOleada;
            int vidaEnemigos = vidaBase + (oleadaActual - 1) * vidaExtraPorOleada;

            Debug.Log($"[WaveSpawner] Oleada {oleadaActual} — {cantidadEnemigos} enemigos | Vel: {velEnemigos:F1} | Vida: {vidaEnemigos}");
            onNuevaOleada?.Invoke(oleadaActual);

            yield return StartCoroutine(SpawnOleada(cantidadEnemigos, velEnemigos, vidaEnemigos));
        }
    }

    IEnumerator SpawnOleada(int cantidad, float velocidad, int vida)
    {
        for (int i = 0; i < cantidad; i++)
        {
            SpawnEnemigo(velocidad, vida);
            yield return new WaitForSeconds(tiempoEntreSpawns);
        }
    }

    void SpawnEnemigo(float velocidad, int vida)
    {
        if (prefabEnemigo == null || puntosSpawn == null || puntosSpawn.Length == 0)
        {
            Debug.LogWarning("[WaveSpawner] Falta prefabEnemigo o puntosSpawn.");
            return;
        }

        // Elegir punto aleatorio
        Transform spawn = puntosSpawn[Random.Range(0, puntosSpawn.Length)];
        GameObject go = Instantiate(prefabEnemigo, spawn.position, spawn.rotation);

        Enemy enemy = go.GetComponent<Enemy>();
        if (enemy == null)
        {
            Debug.LogWarning("[WaveSpawner] El prefab no tiene el componente Enemy.");
            return;
        }

        enemy.Inicializar(canonTransform, velocidad, vida);
        enemigosVivos++;
    }

    /// <summary>Llamado desde Enemy.cs cuando muere un enemigo.</summary>
    public void EnemyMuerto()
    {
        enemigosVivos = Mathf.Max(0, enemigosVivos - 1);
    }
}