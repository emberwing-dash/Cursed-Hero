using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using StarterAssets;

public class PlayerControl : MonoBehaviour
{
    [Space]
    [Header("Components")]
    [SerializeField] private Animator anim;
    [SerializeField] private ThirdPersonController thirdPersonController;

    [Space]
    [Header("Combat")]
    public Transform target;
    [SerializeField] private Transform attackPos;
    [Tooltip("Offset Stopping Distance")][SerializeField] private float quickAttackDeltaDistance;
    [Tooltip("Offset Stopping Distance")][SerializeField] private float heavyAttackDeltaDistance;
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float airknockbackForce = 10f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float reachTime = 0.3f;
    [SerializeField] private LayerMask enemyLayer;
    bool isAttacking = false;

    [Space]
    [Header("Debug")]
    [SerializeField] private bool debug;

    private EnemyBase oldTarget;
    private EnemyBase currentTargetBase;

    void Update()
    {
        HandleInput();
    }

    private void FixedUpdate()
    {
        if (target == null || TargetDetectionControl.instance == null) return;

        if (Vector3.Distance(transform.position, target.position) >= TargetDetectionControl.instance.detectionRange)
        {
            NoTarget();
        }
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.J))
            Attack(0);

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.K))
            Attack(1);
    }

    #region Attack

    public void Attack(int attackState)
    {
        if (isAttacking) return;

        thirdPersonController.canMove = false;
        if (TargetDetectionControl.instance != null)
            TargetDetectionControl.instance.canChangeTarget = false;

        RandomAttackAnim(attackState);
    }

    private void RandomAttackAnim(int attackState)
    {
        switch (attackState)
        {
            case 0: QuickAttack(); break;
            case 1: HeavyAttack(); break;
        }
    }

    void QuickAttack()
    {
        int attackIndex = Random.Range(1, 4);
        if (debug) Debug.Log(attackIndex + " attack index");

        string animName = attackIndex switch
        {
            1 => "punch",
            2 => "kick",
            3 => "mmakick",
            _ => "punch"
        };

        if (target != null)
        {
            MoveTowardsTarget(target.position, quickAttackDeltaDistance, animName);
            isAttacking = true;
        }
        else
        {
            thirdPersonController.canMove = true;
            if (TargetDetectionControl.instance != null)
                TargetDetectionControl.instance.canChangeTarget = true;
        }
    }

    void HeavyAttack()
    {
        int attackIndex = Random.Range(1, 3);
        if (debug) Debug.Log(attackIndex + " heavy attack index");

        string animName = attackIndex == 1 ? "heavyAttack1" : "heavyAttack2";

        if (target != null)
        {
            FaceThis(target.position);
            anim.SetBool(animName, true);
            isAttacking = true;
        }
        else
        {
            thirdPersonController.canMove = true;
            if (TargetDetectionControl.instance != null)
                TargetDetectionControl.instance.canChangeTarget = true;
        }
    }

    public void ResetAttack()
    {
        anim.SetBool("punch", false);
        anim.SetBool("kick", false);
        anim.SetBool("mmakick", false);
        anim.SetBool("heavyAttack1", false);
        anim.SetBool("heavyAttack2", false);

        thirdPersonController.canMove = true;
        if (TargetDetectionControl.instance != null)
            TargetDetectionControl.instance.canChangeTarget = true;

        isAttacking = false;
    }

    public void PerformAttack()
    {
        if (attackPos == null) return;

        Collider[] hitEnemies = Physics.OverlapSphere(attackPos.position, attackRange, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            if (enemy == null) continue;

            Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();
            EnemyBase enemyBase = enemy.GetComponent<EnemyBase>();

            if (enemyRb != null)
            {
                Vector3 knockbackDirection = enemy.transform.position - transform.position;
                knockbackDirection.y = airknockbackForce;
                enemyRb.AddForce(knockbackDirection.normalized * knockbackForce, ForceMode.Impulse);
            }

            if (enemyBase != null)
            {
                enemyBase.SpawnHitVfx(enemyBase.transform.position);
            }
        }
    }

    public void ChangeTarget(Transform target_)
    {
        if (oldTarget != null)
        {
            oldTarget.ActiveTarget(false);
        }

        target = target_;

        if (target_ != null)
        {
            oldTarget = target_.GetComponent<EnemyBase>();
            currentTargetBase = target_.GetComponent<EnemyBase>();

            if (currentTargetBase != null)
                currentTargetBase.ActiveTarget(true);
        }
    }

    private void NoTarget()
    {
        if (currentTargetBase != null)
        {
            currentTargetBase.ActiveTarget(false);
        }

        currentTargetBase = null;
        oldTarget = null;
        target = null;
    }

    #endregion

    #region Movement Helpers

    public void MoveTowardsTarget(Vector3 targetPos, float deltaDistance, string animationName)
    {
        PerformAttackAnimation(animationName);
        FaceThis(targetPos);
        Vector3 finalPos = TargetOffset(targetPos, deltaDistance);
        finalPos.y = 0;
        transform.DOMove(finalPos, reachTime);
    }

    public void GetClose()
    {
        Vector3 getCloseTarget = target != null ? target.position :
                                 oldTarget != null ? oldTarget.transform.position :
                                 transform.position;

        FaceThis(getCloseTarget);
        Vector3 finalPos = TargetOffset(getCloseTarget, 1.4f);
        finalPos.y = 0;
        transform.DOMove(finalPos, 0.2f);
    }

    void PerformAttackAnimation(string animationName)
    {
        anim.SetBool(animationName, true);
    }

    public Vector3 TargetOffset(Vector3 targetPos, float deltaDistance)
    {
        return Vector3.MoveTowards(targetPos, transform.position, deltaDistance);
    }

    public void FaceThis(Vector3 targetPos)
    {
        Vector3 lookPos = new Vector3(targetPos.x, targetPos.y, targetPos.z);
        Quaternion lookRotation = Quaternion.LookRotation(lookPos - transform.position);
        lookRotation.x = 0;
        lookRotation.z = 0;
        transform.DOLocalRotateQuaternion(lookRotation, 0.2f);
    }

    #endregion

    void OnDrawGizmosSelected()
    {
        if (attackPos != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPos.position, attackRange);
        }
    }
}
