using UnityEngine;

public class KillEnemy : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag("Arma"))
        {
           
            Destroy(gameObject);
        }
    }
}