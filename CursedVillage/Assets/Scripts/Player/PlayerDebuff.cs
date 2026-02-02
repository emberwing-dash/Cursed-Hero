using System.Collections;
using UnityEngine;

public class PlayerDebuff : MonoBehaviour
{
    [Header("Debuff VFX")]
    [SerializeField] GameObject debuffVFX;

    [SerializeField] float debuffDuration = 1.5f;

    Coroutine debuffRoutine;


    void Start()
    {
        if (debuffVFX != null)
            debuffVFX.SetActive(false);
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Club"))
            TriggerDebuff();
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Club"))
            TriggerDebuff();
    }


    void TriggerDebuff()
    {
        if (debuffVFX == null) return;

        if (debuffRoutine != null)
            StopCoroutine(debuffRoutine);

        debuffRoutine = StartCoroutine(DebuffRoutine());
    }

    IEnumerator DebuffRoutine()
    {
        debuffVFX.SetActive(true);

        yield return new WaitForSeconds(debuffDuration);

        debuffVFX.SetActive(false);
    }
}
