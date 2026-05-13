using UnityEngine;

public class QuitarVidas : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        
        Vidas.Instance.ActualizarUI();
        Destroy(other.gameObject);
    }
}
