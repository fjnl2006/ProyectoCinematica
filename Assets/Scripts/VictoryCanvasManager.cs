using UnityEngine;

/// <summary>
/// Espera a que todos los <see cref="WaveSpawner"/> configurados terminen su rutina de oleadas
/// y entonces activa el canvas de victoria (útil cuando hay varios managers en distintos puentes).
/// </summary>
public class VictoryCanvasManager : MonoBehaviour
{
    [Header("Victoria")]
    [SerializeField] private GameObject canvasVictoria;
    [SerializeField] private bool pausarAlGanar = true;

    [Header("Oleadas")]
    [Tooltip("Si está vacío, se usan todos los WaveSpawner activos en la escena que tengan oleadas en el ScriptableObject.")]
    [SerializeField] private WaveSpawner[] gestoresOleadas;

    private WaveSpawner[] gestoresResueltos;
    private int totalEsperado;
    private int spawnersFinalizados;
    private bool victoriaMostrada;

    private void Awake()
    {
        if (canvasVictoria != null)
            canvasVictoria.SetActive(false);

        gestoresResueltos = ResolverGestores();
        totalEsperado = gestoresResueltos.Length;
        if (totalEsperado == 0)
            Debug.LogWarning("[VictoryCanvasManager] No hay WaveSpawner con oleadas configuradas. Asigna gestores o revisa la escena.");
    }

    private void OnEnable()
    {
        victoriaMostrada = false;
        spawnersFinalizados = 0;
        if (gestoresResueltos == null) return;

        foreach (WaveSpawner gestor in gestoresResueltos)
        {
            if (gestor != null)
                gestor.onJuegoTerminado += OnUnGestorTermino;
        }
    }

    private void OnDisable()
    {
        if (gestoresResueltos == null) return;

        foreach (WaveSpawner gestor in gestoresResueltos)
        {
            if (gestor != null)
                gestor.onJuegoTerminado -= OnUnGestorTermino;
        }
    }

    private WaveSpawner[] ResolverGestores()
    {
        if (gestoresOleadas != null && gestoresOleadas.Length > 0)
            return gestoresOleadas;

        WaveSpawner[] todos = FindObjectsByType<WaveSpawner>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int conOleadas = 0;
        for (int i = 0; i < todos.Length; i++)
        {
            if (todos[i] != null && todos[i].TieneOleadasConfiguradas())
                conOleadas++;
        }

        if (conOleadas == 0)
            return System.Array.Empty<WaveSpawner>();

        WaveSpawner[] filtrados = new WaveSpawner[conOleadas];
        int j = 0;
        for (int i = 0; i < todos.Length; i++)
        {
            if (todos[i] != null && todos[i].TieneOleadasConfiguradas())
                filtrados[j++] = todos[i];
        }

        return filtrados;
    }

    private void OnUnGestorTermino()
    {
        if (victoriaMostrada) return;

        spawnersFinalizados++;
        if (spawnersFinalizados < totalEsperado) return;

        victoriaMostrada = true;
        if (canvasVictoria != null)
            canvasVictoria.SetActive(true);
        if (pausarAlGanar)
            Time.timeScale = 0f;
    }
}
