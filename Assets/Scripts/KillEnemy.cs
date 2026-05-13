using UnityEngine;

public class KillEnemy : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.gameObject.layer != LayerMask.NameToLayer("Enemy"))
            return;

        if (EnemyHealth.IntentarDaño(other, 99999, "puerta"))
            Debug.Log("La puerta ha aplastado a " + other.gameObject.name);
    }

}