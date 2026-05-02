using UnityEngine;

public class KillEnemy : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Debug.Log("La puerta ha matado a " + other.gameObject.name);
        }
    }

}