using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerCutscene : MonoBehaviour
{
    [System.Serializable]
    public class CutscenePoint
    {
        public Transform point;          // where player moves
        public float duration = 2f;      // how long to stay here
        public AnimType animation;       // which animation to play
        public bool moveToPoint = true;  // move or stand still
    }

    public enum AnimType
    {
        Idle,
        Walk,
        Think,
        Agony
    }

    [Header("Points (order matters)")]
    [SerializeField] private List<CutscenePoint> points = new List<CutscenePoint>();

    [Header("Refs")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;

    private int index = 0;
    private float timer = 0f;

    private CutscenePoint current;

    void Awake()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!animator) animator = GetComponentInChildren<Animator>();

        agent.updateRotation = true;
        agent.isStopped = false;
    }

    void Start()
    {
        if (points.Count == 0) return;

        StartPoint(0);
    }

    void Update()
    {
        if (points.Count == 0) return;

        timer += Time.deltaTime;

        // reached timer → go next
        if (timer >= current.duration)
        {
            NextPoint();
        }
    }


    void StartPoint(int i)
    {
        index = i;
        current = points[index];
        timer = 0f;

        // Movement
        if (current.moveToPoint && current.point != null)
        {
            agent.isStopped = false;
            agent.SetDestination(current.point.position);
        }
        else
        {
            agent.isStopped = true;
        }

        // Animation
        SetAnimation(current.animation);
    }

    void NextPoint()
    {
        if (index + 1 >= points.Count)
        {
            agent.isStopped = true;
            SetAnimation(AnimType.Idle);
            enabled = false;
            return;
        }

        StartPoint(index + 1);
    }


    void SetAnimation(AnimType type)
    {
        // ALWAYS reset first (VERY IMPORTANT)
        animator.SetBool("isWalk", false);
        animator.SetBool("isThink", false);
        animator.SetBool("isAgony", false);

        switch (type)
        {
            case AnimType.Walk:
                animator.SetBool("isWalk", true);
                break;

            case AnimType.Think:
                animator.SetBool("isThink", true);
                break;

            case AnimType.Agony:
                animator.SetBool("isAgony", true);
                break;

            case AnimType.Idle:
            default:
                break;
        }
    }
}
