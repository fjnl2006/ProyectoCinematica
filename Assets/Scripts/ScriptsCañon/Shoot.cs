using UnityEngine;

public class Shoot : MonoBehaviour
{
    private Vector3 velocidadInicial;
    private float gravedad;
    private float tiempoVida;
    private Vector3 spawnPos;
 
    [Header("Explosión")]
    public float radius = 3f;
    public int dañoExplosion = 1;
    public GameObject prefabExplosion;
    [Header("Daño en el área")]
    public float duracionDaño = 0.5f;
 
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
        if (haExplotado) return;
 
        tiempoVida += Time.deltaTime;
        float t = tiempoVida;
 
        float x = velocidadInicial.x * t;
        float y = velocidadInicial.y * t - 0.5f * gravedad * t * t;
        float z = velocidadInicial.z * t;
 
        Vector3 nuevaPos = spawnPos + new Vector3(x, y, z);
 
        // Detectar si hemos cruzado el suelo entre frames (evita atravesar el suelo)
        float alturaActual = transform.position.y;
        float alturaNueva = nuevaPos.y;
 
        if (alturaNueva <= 0.05f && alturaActual > 0.05f)
        {
            // Calcular posición exacta de impacto en Y=0
            float fraccion = alturaActual / (alturaActual - alturaNueva);
            Vector3 impacto = Vector3.Lerp(transform.position, nuevaPos, fraccion);
            impacto.y = 0f;
            transform.position = impacto;
            Explotar();
            return;
        }
 
        transform.position = nuevaPos;
    }
 
    private void OnCollisionEnter(Collision other)
    {
        if (haExplotado) return;
 
        // Por si el collider físico sí detecta el golpe (doble seguridad)
        if (other.gameObject.CompareTag("Floor") || other.gameObject.CompareTag("Ground"))
        {
            transform.position = other.contacts[0].point;
            Explotar();
        }
        else
        {
            Destroy(gameObject);
        }
    }
 
    private void Explotar()
    {
        haExplotado = true;
 
        if (prefabExplosion != null)
        {
            GameObject vfxFuego = Instantiate(prefabExplosion, transform.position, Quaternion.identity);
            FireDamageVFX damageComponent = vfxFuego.GetComponent<FireDamageVFX>();
            if (damageComponent == null)
                damageComponent = vfxFuego.AddComponent<FireDamageVFX>();
 
            damageComponent.Inicializar(radius, dañoExplosion, duracionDaño);
        }
        else
        {
            // Sin VFX: daño directo en área
            AplicarDaño();
        }
 
        Destroy(gameObject);
    }
 
    private void AplicarDaño()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider col in colliders)
            EnemyHealth.IntentarDaño(col, dañoExplosion, "explosion");
    }
 
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.3f, 0f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}