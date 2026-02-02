using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] int maxHealth = 100;

    [Header("UI")]
    [SerializeField] Slider healthSlider;

    int currentHealth;


    void Start()
    {
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
        }
    }

    public int GetHealth()
    {
        return currentHealth;
    }



    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateUI();

        if (currentHealth <= 0)
            Die();
    }


    void UpdateUI()
    {
        if (healthSlider != null)
            healthSlider.value = currentHealth;
    }

    

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Club"))
            TakeDamage(10);
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Club"))
            TakeDamage(10);
    }


    void Die()
    {
        Debug.Log("Player Dead");

        // disable movement / show death UI etc
        // example:
        // GetComponent<PlayerController>().enabled = false;
    }

    public void Heal(int amount)
    {
        Debug.Log("HEAL CALLED on " + gameObject.name);

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("Current HP: " + currentHealth);

        UpdateUI();
    }

}
