using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float velocidad = 3f;

    [Header("Comportamiento Natural")]
    [Tooltip("Amplitud máxima del desvío horizontal")]
    [SerializeField] private float amplitudDesvio = 1.5f;
    [Tooltip("Frecuencia del zig-zag (velocidad de oscilación)")]
    [SerializeField] private float frecuenciaDesvio = 1f;

    private EnemyHealth enemyHealth;
    private Transform objetivo;

    private float desfaseAleatorio;
    private float miFrecuenciaAleatoria;

    private void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
    }

    public void Inicializar(Transform targetTransform, float vel, int hp, WaveSpawner spawnerQueMeCreo)
    {
        objetivo = targetTransform;
        velocidad = vel;
        enemyHealth.Configurar(hp, spawnerQueMeCreo);

        desfaseAleatorio = Random.Range(0f, 100f);
        miFrecuenciaAleatoria = frecuenciaDesvio * Random.Range(0.8f, 1.2f);
    }

    void Update()
    {
        if (enemyHealth.EstaMuerto || objetivo == null) return;

        Vector3 dirBase = (objetivo.position - transform.position);
        dirBase.y = 0f;

        if (dirBase.sqrMagnitude < 0.1f) return;

        Vector3 dirNormalizada = dirBase.normalized;
        Vector3 perpendicular = Vector3.Cross(dirNormalizada, Vector3.up);
        float oscilacion = Mathf.Sin((Time.time * miFrecuenciaAleatoria) + desfaseAleatorio) * amplitudDesvio;
        float atenuacionDistancia = Mathf.Clamp01(dirBase.magnitude / 5f);
        Vector3 dirFinal = dirNormalizada + (perpendicular * oscilacion * atenuacionDistancia * 0.1f);

        transform.position += dirFinal * velocidad * Time.deltaTime;

        if (dirFinal != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(dirFinal.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 8f);
        }
    }
}
