using UnityEngine;

public class CrystalHealth : MonoBehaviour
{
    [SerializeField] public int maxHP = 5;

    int hp;

    public bool IsDead => hp <= 0;

    void Awake()
    {
        hp = maxHP;
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;

        hp -= amount;

        if (hp <= 0)
            Die();
    }

    void Die()
    {
        Destroy(gameObject);
    }

    // Club damage
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Club"))
            TakeDamage(1);
    }
}
