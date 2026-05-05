using UnityEngine;
using UnityEngine.InputSystem;

public class Door : MonoBehaviour
{

    
    [Header("Movimiento")]
    public float velocidad = 2f;
    public float distanciaBajada = 5f;
    public float tiempoEsperaAbajo = 2f;

    private Vector3 posicionInicial;
    private Vector3 posicionFinal;
    private bool enMovimiento = false;

    void Start()
    {
        
        posicionInicial = transform.position;
        posicionFinal = posicionInicial - new Vector3(0, distanciaBajada, 0);
    }

   
    

    public System.Collections.IEnumerator BajarYSubir()
    {
        enMovimiento = true;

        // BAJAR
        while (Vector3.Distance(transform.position, posicionFinal) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, posicionFinal, velocidad * Time.deltaTime);
            yield return null;
        }

        transform.position = posicionFinal;

        // Esperar abajo
        yield return new WaitForSeconds(tiempoEsperaAbajo);

        // SUBIR
        while (Vector3.Distance(transform.position, posicionInicial) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, posicionInicial, velocidad * Time.deltaTime);
            yield return null;
        }

        transform.position = posicionInicial;

        enMovimiento = false;
    }
}
