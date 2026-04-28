using UnityEngine;

public class Shoot : MonoBehaviour
{
    private Vector3 velocidadInicial;
    private float gravedad;
    private float tiempoVida;
    private Vector3 spawnPos;

    [Header("Explosión")]
    public float radius = 3f;
    public int dañoExplosion = 1;           // Daño que aplica la explosión a cada enemigo
    public GameObject prefabExplosion;       // Partículas / VFX (opcional)

    private bool haExplotado = false;

    public void Inicializar(Vector3 velocidad, float gravedad)
    {
        this.velocidadInicial = velocidad;
        this.gravedad = gravedad;
        this.tiempoVida = 0f;
        this.spawnPos = transform.position;
    }

    void Update()
    {
        tiempoVida += Time.deltaTime;
        float t = tiempoVida;

        // Cinemática clásica: pos = pos0 + v0·t + 0.5·a·t²
        float x = velocidadInicial.x * t;
        float y = velocidadInicial.y * t - 0.5f * gravedad * t * t;
        float z = velocidadInicial.z * t;

        transform.position = spawnPos + new Vector3(x, y, z);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (haExplotado) return;
        Explotar();
    }

    private void Explotar()
    {
        haExplotado = true;

        // Partículas opcionales
        if (prefabExplosion != null)
            Instantiate(prefabExplosion, transform.position, Quaternion.identity);

        // Daño en área a todos los enemigos en el radio
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider col in colliders)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null)
                enemy.RecibirDaño(dañoExplosion);
        }

        Destroy(gameObject);
    }

    // Dibuja el radio de explosión en el editor (solo para debug)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.3f, 0f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}