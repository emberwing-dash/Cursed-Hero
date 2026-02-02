using UnityEngine;

public class PlayerBuff : MonoBehaviour
{
    [Header("Direct Reference (drag player here)")]
    [SerializeField] PlayerHealth playerHealth;

    [SerializeField] int healAmount = 2;
    [SerializeField] float healInterval = 1f;
    [SerializeField] GameObject buffVFX;

    float timer;
    bool inside;

    // =====================================================

    void Start()
    {
        if (buffVFX) buffVFX.SetActive(false);
    }

    void Update()
    {
        if (!inside || playerHealth == null)
            return;

        timer += Time.deltaTime;

        if (timer >= healInterval)
        {
            timer = 0f;
            playerHealth.Heal(healAmount);
        }
    }

    // =====================================================

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        inside = true;

        if (buffVFX) buffVFX.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        inside = false;

        if (buffVFX) buffVFX.SetActive(false);
    }
}
