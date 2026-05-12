using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Header("Prefab y punto de spawn")]
    [SerializeField] private GameObject prefabEnemigo;
    [SerializeField] private Transform[] puntosSpawn;
    [SerializeField] private Transform canonTransform;

    [Header("Configuración de oleadas")]
    [SerializeField] private int enemigosBase = 3;
    [SerializeField] private int extraPorOleada = 2;
    [SerializeField] private float tiempoEntreOleadas = 5f;
    [SerializeField] private float tiempoEntreSpawns = 0.5f;

    [Header("Escalado de dificultad")]
    [SerializeField] private float velocidadBase = 3f;
    [SerializeField] private float velocidadExtraPorOleada = 0.3f;
    [SerializeField] private int vidaBase = 3;
    [SerializeField] private int vidaExtraPorOleada = 1;

    private int oleadaActual = 0;
    private int enemigosVivos = 0;
    private bool esperando = false;

    public System.Action<int> onNuevaOleada;
    public System.Action onJuegoTerminado;

    // ELIMINADO: public static WaveSpawner instance; y el método Awake()

    void Start()
    {
        StartCoroutine(RutinaOleadas());
    }

    IEnumerator RutinaOleadas()
    {
        while (true)
        {
            if (oleadaActual > 0)
            {
                esperando = true;
                yield return new WaitUntil(() => enemigosVivos <= 0);
                esperando = false;
                yield return new WaitForSeconds(tiempoEntreOleadas);
            }

            oleadaActual++;
            int cantidadEnemigos = enemigosBase + (oleadaActual - 1) * extraPorOleada;
            float velEnemigos = velocidadBase + (oleadaActual - 1) * velocidadExtraPorOleada;
            int vidaEnemigos = vidaBase + (oleadaActual - 1) * vidaExtraPorOleada;

            Debug.Log($"[WaveSpawner {gameObject.name}] Oleada {oleadaActual} — {cantidadEnemigos} enemigos");
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
        if (prefabEnemigo == null || puntosSpawn == null || puntosSpawn.Length == 0) return;

        Transform spawn = puntosSpawn[Random.Range(0, puntosSpawn.Length)];
        GameObject go = Instantiate(prefabEnemigo, spawn.position, spawn.rotation);

        Enemy enemy = go.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.Inicializar(canonTransform, velocidad, vida);
            enemigosVivos++;
        }
    }

    public void EnemyMuerto()
    {
        enemigosVivos = Mathf.Max(0, enemigosVivos - 1);
    }
}