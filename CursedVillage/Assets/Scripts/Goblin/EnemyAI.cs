using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] private List<Transform> smallCrystals;
    [SerializeField] private Transform mainCrystal;
    [SerializeField] private Transform player;

    [Header("Movement & Combat")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 4.5f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1.2f;
    [SerializeField] private float attackDuration = 0.8f;

    [Header("Audio")]
    [SerializeField] private AudioSource spotAudio;

    [Header("Death VFX")]
    [SerializeField] private GameObject deathVfxPrefab;

    private NavMeshAgent agent;
    private Animator animator;

    private Transform currentTarget;
    private Transform currentCrystal;

    private float attackTimer;
    private bool isAttacking;
    private bool playerDetected = false;
    private bool hasPlayedSpotAudio = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        agent.updateRotation = false;

        // Start by targeting crystals
        ChooseTarget();
    }

    void Update()
    {
        attackTimer += Time.deltaTime;

        // Update target
        ChooseTarget();

        if (currentTarget == null)
        {
            agent.isStopped = true;
            animator.SetBool("isWalk", false);
            animator.SetBool("isRun", false);
            return;
        }

        float dist = Vector3.Distance(transform.position, currentTarget.position);

        if (dist > attackRange)
        {
            StopAttack();

            agent.isStopped = false;
            agent.speed = (currentTarget == player) ? runSpeed : walkSpeed;
            agent.SetDestination(currentTarget.position);

            animator.SetBool("isRun", currentTarget == player);
            animator.SetBool("isWalk", currentTarget != player);
        }
        else
        {
            if (!isAttacking)
                StartCoroutine(AttackRoutine());
        }

        FaceTarget();
    }

    void ChooseTarget()
    {
        // Player priority if detected
        if (playerDetected && player != null && player.gameObject.activeInHierarchy)
        {
            currentTarget = player;
            return;
        }

        // Nearest alive small crystal
        currentCrystal = GetNearestCrystal();
        if (currentCrystal != null)
        {
            currentTarget = currentCrystal;
            return;
        }

        // No small crystals left, target main crystal
        if (mainCrystal != null)
        {
            CrystalHealth ch = mainCrystal.GetComponent<CrystalHealth>();
            if (ch != null && !ch.IsDead)
                currentTarget = mainCrystal;
            else
                currentTarget = null; // nothing to attack
        }
    }

    Transform GetNearestCrystal()
    {
        float bestDist = Mathf.Infinity;
        Transform nearest = null;

        foreach (var c in smallCrystals)
        {
            if (c == null) continue;

            CrystalHealth ch = c.GetComponent<CrystalHealth>();
            if (ch != null && ch.IsDead) continue;

            float d = Vector3.Distance(transform.position, c.position);
            if (d < bestDist)
            {
                bestDist = d;
                nearest = c;
            }
        }

        return nearest;
    }

    IEnumerator AttackRoutine()
    {
        if (attackTimer < attackCooldown) yield break;

        isAttacking = true;
        attackTimer = 0f;

        agent.isStopped = true;
        animator.SetBool("isWalk", false);
        animator.SetBool("isRun", false);

        bool atk1 = Random.value > 0.5f;
        animator.SetBool("atk1", atk1);
        animator.SetBool("atk2", !atk1);

        yield return new WaitForSeconds(attackDuration * 0.4f);

        // Deal damage
        if (currentTarget != null)
        {
            if (currentTarget.CompareTag("Player"))
            {
                var ph = currentTarget.GetComponent<PlayerHealth>();
                if (ph != null)
                    ph.TakeDamage(10);
            }
            else if (currentTarget.CompareTag("Crystal") || currentTarget.CompareTag("MainCrystal"))
            {
                var ch = currentTarget.GetComponent<CrystalHealth>();
                if (ch != null)
                {
                    ch.TakeDamage(1);
                    if (ch.IsDead && currentTarget == currentCrystal)
                        currentCrystal = null;
                }
            }
        }

        yield return new WaitForSeconds(attackDuration * 0.6f);

        StopAttack();
    }

    void StopAttack()
    {
        StopAllCoroutines();
        animator.SetBool("atk1", false);
        animator.SetBool("atk2", false);
        isAttacking = false;
    }

    void FaceTarget()
    {
        if (currentTarget == null) return;

        Vector3 dir = currentTarget.position - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude < 0.01f) return;

        float rotSpeed = isAttacking ? 20f : 10f;
        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotSpeed * Time.deltaTime);
    }

    public void KillGoblin()
    {
        StopAttack();
        agent.isStopped = true;
        animator.enabled = false;

        if (deathVfxPrefab != null)
        {
            GameObject vfx = Instantiate(deathVfxPrefab, transform.position, Quaternion.identity);
            Destroy(vfx, 2f);
        }

        gameObject.SetActive(false);
    }

    // --- Player Detection via Child Trigger ---
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerDetected = true;

            if (!hasPlayedSpotAudio && spotAudio != null)
            {
                spotAudio.Play();
                hasPlayedSpotAudio = true;
            }
        }
    }
}
