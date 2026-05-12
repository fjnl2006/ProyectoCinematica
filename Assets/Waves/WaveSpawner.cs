using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Header("Prefab y punto de spawn")]
    [SerializeField] private GameObject prefabEnemigo;
    [SerializeField] private Transform[] puntosSpawn;
    [SerializeField] private Transform canonTransform;

    [Header("Oleadas")]
    [SerializeField] private List<WaveConfig> oleadas;

    private int oleadaActual = 0;
    private int enemigosVivos = 0;
    private bool esperando = false;

    public System.Action<int> onNuevaOleada;
    public System.Action onJuegoTerminado;

    void Start()
    {
        StartCoroutine(RutinaOleadas());
    }

    IEnumerator RutinaOleadas()
    {
        while (oleadaActual < oleadas.Count)
        {
            if (oleadaActual > 0)
            {
                esperando = true;
                yield return new WaitUntil(() => enemigosVivos <= 0);
                esperando = false;
                yield return new WaitForSeconds(oleadas[oleadaActual].tiempoEntreSpawns);
            }

            var config = oleadas[oleadaActual];
            oleadaActual++;

            Debug.Log($"[WaveSpawner {gameObject.name}] Oleada {oleadaActual} — {config.cantidadEnemigos} enemigos");
            onNuevaOleada?.Invoke(oleadaActual);

            yield return StartCoroutine(SpawnOleada(config));
        }
    }

    IEnumerator SpawnOleada(WaveConfig config)
    {
        for (int i = 0; i < config.cantidadEnemigos; i++)
        {
            SpawnEnemigo(config);
            yield return new WaitForSeconds(config.tiempoEntreSpawns);
        }
    }

    void SpawnEnemigo(WaveConfig config)
    {
        if (prefabEnemigo == null || puntosSpawn == null || puntosSpawn.Length == 0) return;

        Transform spawn = puntosSpawn[Random.Range(0, puntosSpawn.Length)];
        GameObject go = Instantiate(prefabEnemigo, spawn.position, spawn.rotation);

        // Aplica un ángulo aleatorio
        float angulo = Random.Range(-config.anguloDesviacion, config.anguloDesviacion);
        go.transform.Rotate(0, angulo, 0);

        Enemy enemy = go.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.Inicializar(canonTransform, config.velocidadEnemigos, config.vidaEnemigos);
            enemigosVivos++;
        }
    }

    public void EnemyMuerto()
    {
        enemigosVivos = Mathf.Max(0, enemigosVivos - 1);
    }
}