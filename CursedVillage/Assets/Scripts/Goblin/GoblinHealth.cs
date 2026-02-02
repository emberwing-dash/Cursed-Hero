using UnityEngine;

public class GoblinHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] int maxHealth = 15;

    [Header("Death VFX")]
    [SerializeField] GameObject deathVfxPrefab;

    int currentHealth;
    bool dead;

    public bool IsDead => dead;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    // SAFE DAMAGE (can be called from ANY child collider)
    public void TakeDamage(int dmg)
    {
        if (dead) return;

        currentHealth -= dmg;

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        if (dead) return;
        dead = true;

        // spawn VFX
        if (deathVfxPrefab != null)
        {
            GameObject vfx = Instantiate(
                deathVfxPrefab,
                transform.position,
                Quaternion.identity
            );

            Destroy(vfx, 2f);
        }

        // disable WHOLE goblin root (AI + NavMesh + everything)
        gameObject.SetActive(false);
    }
}
