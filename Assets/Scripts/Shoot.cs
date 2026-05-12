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
    [Header("Daño en el área")] 
    public float duracionDaño = 0.5f;
    
    private bool haExplotado = false;
    private Vector3 ultimaPosicionColision;
    
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
        
        // Solo explota si colisiona con el suelo (tag "Ground")
        if (other.gameObject.CompareTag("Ground"))
        {
            ultimaPosicionColision = transform.position;
            Explotar();
        }
        else
        {
            // Si no es suelo, solo destruye la bala sin daño
            Destroy(gameObject);
        }
    }

    private void Explotar()
    {
        haExplotado = true;
        // Partículas opcionales
        if (prefabExplosion != null)
        {
            GameObject vfxFuego = Instantiate(prefabExplosion, ultimaPosicionColision, Quaternion.identity);
            
            FireDamageVFX damageComponent = vfxFuego.AddComponent<FireDamageVFX>();
            damageComponent.Inicializar(radius, dañoExplosion, duracionDaño);
        }
        else
        {
            AplicarDaño();
        }

        // Daño en área a todos los enemigos en el radio
        

        Destroy(gameObject);
    }

    private void AplicarDaño()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider col in colliders)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null)
                enemy.RecibirDaño(dañoExplosion);
        }
    }

    // Dibuja el radio de explosión en el editor (solo para debug)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.3f, 0f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}