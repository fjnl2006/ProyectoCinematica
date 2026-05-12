using UnityEngine;

public class ChangeCamera : MonoBehaviour
{
    [SerializeField] private GameObject[] camarasDelPuente;
    private int indiceActual = 0;

    // Llama a esto cuando pulses el botón de cambiar cámara
    public void CiclarCamara()
    {
        camarasDelPuente[indiceActual].SetActive(false);
        indiceActual = (indiceActual + 1) % camarasDelPuente.Length;
        camarasDelPuente[indiceActual].SetActive(true);
    }

    // El BridgeManager llamará a esto al entrar al puente
    public void ActivarSistema()
    {
        camarasDelPuente[indiceActual].SetActive(true);
    }

    // El BridgeManager llamará a esto al salir a la vista cenital
    public void DesactivarSistema()
    {
        foreach (var cam in camarasDelPuente)
        {
            cam.SetActive(false);
        }
    }
}