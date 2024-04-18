using UnityEngine;

public class DamageGiver : MonoBehaviour
{
    public int damageAmount = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tank"))
        {
            other.GetComponent<TankInfo>().TakeDamage(damageAmount);
            Destroy(gameObject); // Destroy the projectile after hitting the tank
        }
    }
}
