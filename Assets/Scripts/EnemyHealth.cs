using UnityEngine;

/// <summary>
/// Vida compartida del enemigo: todo el daño debe pasar por aquí (bala, fuego, puerta, etc.).
/// </summary>
[DisallowMultipleComponent]
public class EnemyHealth : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int vidaInicial = 3;

    private int vidaActual;
    private bool muerto;
    private WaveSpawner miSpawner;

    public int VidaActual => vidaActual;
    public bool EstaMuerto => muerto;

    private void Awake()
    {
        if (vidaActual <= 0 && !muerto)
            vidaActual = vidaInicial;
    }

    /// <summary>
    /// Oleada u otro sistema fija la vida al nacer.
    /// </summary>
    public void Configurar(int hp, WaveSpawner spawner)
    {
        miSpawner = spawner;
        muerto = false;
        vidaActual = Mathf.Max(1, hp);
    }

    /// <summary>
    /// Daño desde colliders hijos o padre: usa el mismo pool de vida.
    /// </summary>
    public static bool IntentarDaño(Component colisionOtro, int cantidad, string fuente = "desconocido")
    {
        if (colisionOtro == null || cantidad <= 0) return false;
        EnemyHealth salud = colisionOtro.GetComponentInParent<EnemyHealth>();
        if (salud == null || salud.EstaMuerto) return false;
        salud.RecibirDaño(cantidad, fuente);
        return true;
    }

    public void RecibirDaño(int cantidad = 1, string fuente = "desconocido")
    {
        if (muerto || cantidad <= 0) return;
        vidaActual -= cantidad;
        if (vidaActual <= 0)
            Morir(fuente);
    }

    private void Morir(string fuente = "desconocido")
    {
        if (muerto) return;
        muerto = true;

        if (miSpawner != null)
            miSpawner.EnemyMuerto();

        Destroy(gameObject);
    }
}
