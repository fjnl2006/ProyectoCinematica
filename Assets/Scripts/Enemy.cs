using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int vida = 3;
    [SerializeField] private float velocidad = 3f;

    [Header("Referencia al cañón")]
    [SerializeField] private Transform objetivo; // Se asigna por código desde el spawner

    private bool muerto = false;

    // Referencia al WaveSpawner para notificar cuando el enemigo muere(metodo de abajo)
    private WaveSpawner waveSpawner;

    public void Inicializar(Transform targetCannon, float vel = 3f, int hp = 3, WaveSpawner spawner = null)
    {
        objetivo = targetCannon;
        velocidad = vel;
        vida = hp;
        waveSpawner = spawner; // Asigna su propio wave spwaner por si hacemos mas de 1
    }

    void Update()
    {
        if (muerto || objetivo == null) return;

        // Moverse hacia el cañón en el plano XZ
        Vector3 dir = (objetivo.position - transform.position);
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.1f) return; // Ya llegó

        transform.position += dir.normalized * velocidad * Time.deltaTime;

        // Rotar mirando hacia el cañón
        Quaternion rot = Quaternion.LookRotation(dir.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 8f);
    }

    public void RecibirDaño(int cantidad = 1)
    {
        if (muerto) return;
        vida -= cantidad;
        if (vida <= 0) Morir();
    }

    private void Morir()
    {
        muerto = true;

        // Avisa al spawner que el enemigo ha muerto
        if (waveSpawner != null)
        {
            waveSpawner.EnemyMuerto();
        }

        Destroy(gameObject);
    }
}