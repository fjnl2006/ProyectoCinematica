using NUnit.Framework;
using System.Text;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlPuerta : MonoBehaviour
{
    [Header("Componentes")]
    public HingeJoint bisagraPuerta;

    [Header("Ajustes de Movimiento")]
    public float velocidadSubida = 100f;
    public float velocidadBajada = 50f;
    public float fuerzaMotor = 1000f;

    private JointMotor motor;

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
        if (bisagraPuerta == null) return;

        if (context.started || context.performed)
        {
            ConfigurarMotor(-velocidadSubida);
        }
        else if (context.canceled)
        {
            ConfigurarMotor(velocidadBajada);
        }
    }

    private void ConfigurarMotor(float velocidad)
    {
        motor.targetVelocity = velocidad;
        motor.force = fuerzaMotor;
        bisagraPuerta.motor = motor;
    }
}