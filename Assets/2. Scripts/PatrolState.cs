using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PatrolState : IState
{
    private Monster monster;

    private Vector3 patrolTarget;

    public PatrolState(Monster monster)
    {
        this.monster = monster;
    }
    public void Enter()
    {
        monster.anim.SetFloat("Blend", 1f);
        SetRandomPatrolTarget();
    }
    public void Update() 
    {
        // 1) 플레이어 탐지 → 추적 상태로 전환
        if (monster.canTracePlayer)
        {
            monster.ChangeState(new TraceState(monster));
            return;
        }

        // 2) patrolTarget 방향으로 회전
        Vector3 dir = patrolTarget - monster.transform.position;
        dir.y = 0f;

        if (dir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            monster.transform.rotation =
                Quaternion.Slerp(monster.transform.rotation, targetRot, 10f * Time.deltaTime);
        }

        // 3) patrolTarget까지 이동
        monster.transform.position =
            Vector3.MoveTowards(
                monster.transform.position,
                patrolTarget,
                monster.moveSpeed * Time.deltaTime);

        // 4) 목적지 도달 → 새로운 목적지 설정
        if (Vector3.Distance(monster.transform.position, patrolTarget) < 0.2f)
        {
            SetRandomPatrolTarget();
        }
    }
    public void Exit() { }
    
    private void SetRandomPatrolTarget()
    {
        Vector2 rand = Random.insideUnitCircle * monster.patrolRadius;
        patrolTarget = monster.homePosition + new Vector3(rand.x, 0f, rand.y);
    }
}


