using System;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    private Vector3 velocidadInicial;
    private float gravedad;
    private float tiempoVida;
    private Vector3 spawnPos;
    
    Vector3 exploxionPos;
    public float radius;
    
    public void Inicializar(Vector3 velocidad, float gravedad)
    {
        this.velocidadInicial = velocidad;
        this.gravedad = gravedad;
        this.tiempoVida = 0f;
        this.spawnPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        tiempoVida += Time.deltaTime;
        float t = tiempoVida;

        // Cinemática clásica: pos = pos0 + v0*t + 0.5*a*t²
        float x = velocidadInicial.x * t;
        float y = velocidadInicial.y * t - 0.5f * gravedad * t * t;
        float z = velocidadInicial.z * t;

        transform.position = spawnPos + new Vector3(x, y, z);
    }

    private void OnCollisionEnter(Collision other)
    {
        Collider[] colliders = Physics.OverlapSphere(exploxionPos, radius);
        for (int i = 0; i < colliders.Length; i++)
        {
            Destroy(colliders[i].gameObject);
        }
        
        if (other.gameObject.tag == "Enemy")
        {
            
        }
    }
}
