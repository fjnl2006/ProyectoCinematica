using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int vida = 3;
    [SerializeField] private float velocidad = 3f;

    [Header("Comportamiento Natural")]
    [Tooltip("Amplitud máxima del desvío horizontal")]
    [SerializeField] private float amplitudDesvio = 1.5f;
    [Tooltip("Frecuencia del zig-zag (velocidad de oscilación)")]
    [SerializeField] private float frecuenciaDesvio = 1f;

    private Transform objetivo;
    private WaveSpawner miSpawner;
    private bool muerto = false;

    // Variables internas para el movimiento natural
    private float desfaseAleatorio;
    private float miFrecuenciaAleatoria;

    public void Inicializar(Transform targetTransform, float vel, int hp, WaveSpawner spawnerQueMeCreo)
    {
        objetivo = targetTransform;
        velocidad = vel;
        vida = hp;
        miSpawner = spawnerQueMeCreo;

        // Cada enemigo calcula su propio ritmo y lado de desvío al nacer
        desfaseAleatorio = Random.Range(0f, 100f);
        miFrecuenciaAleatoria = frecuenciaDesvio * Random.Range(0.8f, 1.2f);
    }

    void Update()
    {
        if (muerto || objetivo == null) return;

        // 1. Calcular la dirección base hacia el objetivo
        Vector3 dirBase = (objetivo.position - transform.position);
        dirBase.y = 0f;

        if (dirBase.sqrMagnitude < 0.1f) return;

        Vector3 dirNormalizada = dirBase.normalized;

        // 2. Calcular un vector perpendicular (para el desvío a izquierda o derecha)
        Vector3 perpendicular = Vector3.Cross(dirNormalizada, Vector3.up);

        // 3. uN SENO para la variabilidad y que no sea una fila de enemigos tan predecible, con un desfase aleatorio para que no estén sincronizados
        float oscilacion = Mathf.Sin((Time.time * miFrecuenciaAleatoria) + desfaseAleatorio) * amplitudDesvio;

        // 4. Suavizar la oscilación a medida que se acercan al cañón para que puedan ser aplastados/disparados bien
        float atenuacionDistancia = Mathf.Clamp01(dirBase.magnitude / 5f);

        // 5. La dirección final es una mezcla de avanzar hacia adelante y oscilar hacia los lados
        Vector3 dirFinal = dirNormalizada + (perpendicular * oscilacion * atenuacionDistancia * 0.1f);

        // Mover
        transform.position += dirFinal * velocidad * Time.deltaTime;

        // Rotar mirando hacia donde caminan realmente
        if (dirFinal != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(dirFinal.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 8f);
        }
    }

    public void RecibirDaño(int cantidad = 1, string fuente = "desconocido")
    {
        if (muerto) return;
        vida -= cantidad;
        if (vida <= 0) Morir(fuente);
    }

    private void Morir(string fuente = "desconocido")
    {
        muerto = true;

        if (miSpawner != null)
        {
            miSpawner.EnemyMuerto();
        }

        Destroy(gameObject);
    }
}