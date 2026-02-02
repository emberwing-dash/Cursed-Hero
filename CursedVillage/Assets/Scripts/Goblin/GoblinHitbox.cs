using UnityEngine;

public class GoblinHitbox : MonoBehaviour
{
    [SerializeField] GoblinHealth health;
    [SerializeField] int swordDamage = 3;

    void Awake()
    {
        if (health == null)
            health = GetComponentInParent<GoblinHealth>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sword"))
        {
            health.TakeDamage(swordDamage);
        }
    }
}
