using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class ControlPuerta : MonoBehaviour
{
    [Header("Componentes")]
    public HingeJoint bisagraPuerta;

    [Header("Configuración de Tiempos")]
    public float tiempoArriba = 2f;
    public float tiempoEspera = 5f;

    [Header("Ajustes de Motor")]
    public float velocidadSubida = 150f;
    public float velocidadBajada = 50f;
    public float fuerzaMotor = 2000f;

    private JointMotor motor;
    private bool puedeActivar = true;

    void Start()
    {
        if (bisagraPuerta != null)
        {
            motor = bisagraPuerta.motor;
            bisagraPuerta.useMotor = true;
        }
    }

    public void OnMoverPuerta(InputAction.CallbackContext context)
    {
        if (context.started && puedeActivar)
        {
            StartCoroutine(SecuenciaPuerta());
        }
    }

    IEnumerator SecuenciaPuerta()
    {
        puedeActivar = false;

        ConfigurarMotor(-velocidadSubida);

        float anguloObjetivo = bisagraPuerta.limits.min + 2f;

        while (bisagraPuerta.angle > anguloObjetivo)
        {
            yield return new WaitForFixedUpdate();
        }

        Debug.Log("Límite alcanzado. Esperando tiempo de cortesía...");
        yield return new WaitForSeconds(tiempoArriba);

        ConfigurarMotor(velocidadBajada);

        yield return new WaitForSeconds(tiempoEspera);

        puedeActivar = true;
        Debug.Log("Sistema listo");
    }

    private void ConfigurarMotor(float velocidad)
    {
        motor.targetVelocity = velocidad;
        motor.force = fuerzaMotor;
        bisagraPuerta.motor = motor;
    }
}