using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("추적 세팅")]
    public PlayerMove tracingTarget;
    public float moveSpeed = 3.0f;
    public float tracingRange = 5.0f;
    public float attackRange = 1.0f;
    public float patrolRadius = 8.0f;
    public Vector3 homePosition;           //복귀지점

    public bool canTracePlayer;            //추적 가능 여부
    public bool canAttackPlayer;           //공격 가능 여부
    public bool isHomePoint;               //복귀지점에 도달했는지
    public bool isAttacking;
    public Animator anim;
    public IState currentState;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    private void Start()
    {
        homePosition = transform.position;
        currentState = new PatrolState(this);
        currentState.Enter();
    }
    private void Update()
    {
        isHomePoint = Vector3.Distance(transform.position, homePosition) < 0.2f;

        float distance = Vector3.Distance(transform.position, tracingTarget.transform.position);

        canTracePlayer = distance <= tracingRange; //둘 사이의 거리가 추격범위보다 작거나 같다면
        canAttackPlayer = distance <= attackRange; //둘 사이의 거리가 공격범위보다 작거나 같다면

        currentState?.Update();

    }
    public void ChangeState(IState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();

    }
    private void OnDrawGizmosSelected()
    {
        // 패트롤 범위 (homePosition 기준)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(homePosition, patrolRadius);

        // 추적 범위 (현재 위치 기준)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, tracingRange);

        // 공격 범위 (현재 위치 기준)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public void RealAttack()
    {
        if (tracingTarget == null) return;

        float distance = Vector3.Distance(transform.position, tracingTarget.transform.position);

        if (distance <= attackRange)
        {
            tracingTarget.TakeHit();
        }
    }
    public void AttackAnimationEnd()
    {
        isAttacking = false;
    }

}
