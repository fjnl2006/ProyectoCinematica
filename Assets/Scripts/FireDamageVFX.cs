using UnityEngine;

public class FireDamageVFX : MonoBehaviour
{
    private float radius;
    private int daño;
    private float duracionDaño;
    private SphereCollider damageCollider;
    
    public void Inicializar(float radiusExplosion, int dañoExplosion, float duracion)
    {
        this.radius = radiusExplosion;
        this.daño = dañoExplosion;
        this.duracionDaño = duracion;

        damageCollider = gameObject.AddComponent<SphereCollider>();
        damageCollider.isTrigger = true;
        damageCollider.radius = radius;
    }
    private void OnTriggerEnter(Collider other)
    {
        // Si un enemigo entra en el área de fuego durante la duración, recibe daño
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.RecibirDaño(daño);
        }
    }
    private void AplicarDaño()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider col in colliders)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.RecibirDaño(daño);
            }
        }
    }


}
