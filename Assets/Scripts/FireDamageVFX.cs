using UnityEngine;

public class FireDamageVFX : MonoBehaviour
{
    private float radius;
    private int daño;
    private float duracionDaño;
    private SphereCollider damageCollider;
    private bool inicializado = false;
 
    public void Inicializar(float radiusExplosion, int dañoExplosion, float duracion)
    {
        this.radius = radiusExplosion;
        this.daño = dañoExplosion;
        this.duracionDaño = duracion;
 
        damageCollider = gameObject.AddComponent<SphereCollider>();
        damageCollider.isTrigger = true;
        damageCollider.radius = radius;
        damageCollider.center = new Vector3(0f, radius * 0.5f, 0f); // Eleva el centro para cubrir la altura del enemigo
 
        inicializado = true;
 
        // Daño instantáneo a todo lo que ya esté en el área al explotar
        AplicarDañoInstantaneo();
 
        // Destruir el VFX tras la duración
        Destroy(gameObject, duracion);
    }
 
    private void OnTriggerEnter(Collider other)
    {
        if (!inicializado) return;
 
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            Debug.Log($"[FireDamageVFX] OnTriggerEnter — {other.gameObject.name} recibe {daño} de daño");
            enemy.RecibirDaño(daño, "fuego");
        }
    }
 
    private void AplicarDañoInstantaneo()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider col in colliders)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null)
            {
                Debug.Log($"[FireDamageVFX] DañoInstantaneo — {col.gameObject.name} recibe {daño} de daño");
                enemy.RecibirDaño(daño, "fuego");
            }
        }
    }


}
