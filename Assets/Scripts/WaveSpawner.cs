using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Header("Prefab y punto de spawn")]
    [SerializeField] private GameObject prefabEnemigo;
    [SerializeField] private Transform[] puntosSpawn;
    [SerializeField] private Transform objetivoDeEstePuente;

    [Header("Datos de Oleadas (Scriptable Object)")]
    [SerializeField] private LevelWavesSO configuracionOleadas;

    [Header("Variabilidad (Desincronización)")]
    [Tooltip("Retraso máximo al empezar la primera oleada para que los puentes no vayan a la vez")]
    [SerializeField] private float maxRetrasoInicial = 2f;
    [Tooltip("Variación de tiempo (+/-) al instanciar cada enemigo")]
    [SerializeField] private float variacionSpawn = 0.2f;

    private int indiceOleadaActual = 0;
    private int enemigosVivos = 0;
    private bool esperando = false;

    public System.Action<int> onNuevaOleada;
    public System.Action onJuegoTerminado;

    void Start()
    {
        if (configuracionOleadas != null && configuracionOleadas.oleadas.Length > 0)
        {
            StartCoroutine(RutinaOleadas());
        }
    }

    IEnumerator RutinaOleadas()
    {
        // 1. Variabilidad: Retraso inicial aleatorio para desincronizar los 4 puentes
        float retrasoInicial = Random.Range(0f, maxRetrasoInicial);
        yield return new WaitForSeconds(retrasoInicial);

        while (indiceOleadaActual < configuracionOleadas.oleadas.Length)
        {
            if (indiceOleadaActual > 0)
            {
                esperando = true;
                yield return new WaitUntil(() => enemigosVivos <= 0);
                esperando = false;
                yield return new WaitForSeconds(configuracionOleadas.tiempoEntreOleadas);
            }

            DatosOleada oleadaActual = configuracionOleadas.oleadas[indiceOleadaActual];
            onNuevaOleada?.Invoke(indiceOleadaActual + 1);

            yield return StartCoroutine(SpawnOleada(oleadaActual));

            indiceOleadaActual++;
        }
        onJuegoTerminado?.Invoke();
    }

    IEnumerator SpawnOleada(DatosOleada datos)
    {
        for (int i = 0; i < datos.cantidadEnemigos; i++)
        {
            SpawnEnemigo(datos.velocidadEnemigos, datos.vidaEnemigos);

            // 2. Variabilidad: Tiempo entre enemigos ligeramente aleatorio
            float tiempoAleatorio = datos.tiempoEntreSpawns + Random.Range(-variacionSpawn, variacionSpawn);
            yield return new WaitForSeconds(Mathf.Max(0.1f, tiempoAleatorio));
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
            enemy.Inicializar(objetivoDeEstePuente, velocidad, vida, this);
            enemigosVivos++;
        }
    }

    public void EnemyMuerto()
    {
        enemigosVivos = Mathf.Max(0, enemigosVivos - 1);
    }
}