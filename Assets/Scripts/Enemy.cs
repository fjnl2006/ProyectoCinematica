using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int vida = 3;
    [SerializeField] private float velocidad = 3f;
 
    [Header("Referencia al cañón")]
    [SerializeField] private Transform objetivo;
 
    private bool muerto = false;
 
    public void Inicializar(Transform targetCannon, float vel = 3f, int hp = 3)
    {
        objetivo = targetCannon;
        velocidad = vel;
        vida = hp;
    }
 
    void Update()
    {
        if (muerto || objetivo == null) return;
 
        Vector3 dir = (objetivo.position - transform.position);
        dir.y = 0f;
 
        if (dir.sqrMagnitude < 0.1f) return;
 
        transform.position += dir.normalized * velocidad * Time.deltaTime;
 
        Quaternion rot = Quaternion.LookRotation(dir.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 8f);
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
 
        if (fuente == "fuego")
            Debug.Log($"[Enemy] {gameObject.name} ha muerto por FUEGO");
        else
            Debug.Log($"[Enemy] {gameObject.name} ha muerto por {fuente}");
 
        Destroy(gameObject);
    }
}