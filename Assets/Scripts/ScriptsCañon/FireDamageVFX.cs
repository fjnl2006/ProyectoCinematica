using System.Collections.Generic;
using UnityEngine;

public class FireDamageVFX : MonoBehaviour
{
    private float radius;
    private int daño;
    private SphereCollider damageCollider;
    private bool inicializado = false;
    private readonly HashSet<EnemyHealth> enemigosYaDañados = new HashSet<EnemyHealth>();
 
    public void Inicializar(float radiusExplosion, int dañoExplosion, float duracion)
    {
        this.radius = radiusExplosion;
        this.daño = dañoExplosion;

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
        AplicarDañoA(other);
    }

    private void AplicarDañoInstantaneo()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider col in colliders)
            AplicarDañoA(col);
    }

    private void AplicarDañoA(Collider col)
    {
        EnemyHealth salud = col.GetComponentInParent<EnemyHealth>();
        if (salud == null || salud.EstaMuerto) return;
        if (!enemigosYaDañados.Add(salud)) return;

        Debug.Log($"[FireDamageVFX] {col.gameObject.name} recibe {daño} de daño (vida compartida)");
        salud.RecibirDaño(daño, "fuego");
    }


}
