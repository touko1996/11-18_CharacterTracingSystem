using System.Collections;
using UnityEngine;

public class AttackState : IState
{
    private Monster monster;
    private Coroutine coAttack;

    public AttackState(Monster monster)
    {
        this.monster = monster;
    }

    public void Enter()
    {
        // 공격 시작
        monster.isAttacking = true;
        // 이동 애니메이션 끄기
        monster.anim.SetFloat("Blend", 0f);

        // 첫 공격은 즉시 실행
        monster.anim.SetTrigger("Attack");

        // 반복 공격 루틴 시작
        coAttack = monster.StartCoroutine(CoAttack());
    }

    public void Update()
    {
        // 공격 중에 상태 전환 금지
        if (monster.isAttacking)
        {
            RotateToPlayer();
            return;
        }

        
        // 공격 범위를 벗어난 경우 추적 상태로 복귀
        if (!monster.canAttackPlayer)
        {
            monster.ChangeState(new TraceState(monster));
            return;
        }

        // 공격 중 아니지만 공격 사거리라면 회전 유지
        RotateToPlayer();
    }

    public void Exit()
    {
        monster.isAttacking = false;

        if (coAttack != null)
            monster.StopCoroutine(coAttack);
    }

    private IEnumerator CoAttack()
    {
        while (true)
        {
            monster.isAttacking = true;

            monster.anim.SetTrigger("Attack");

            // 공격 딜레이 (모션 + 쿨타임)
            yield return new WaitForSeconds(1.5f);

            monster.isAttacking = false;

            // 공격 끝났고 공격 범위가 아니라면 추적으로 이동
            if (!monster.canAttackPlayer)
            {
                monster.ChangeState(new TraceState(monster));
                yield break;
            }
        }
    }

    private void RotateToPlayer()
    {
        Vector3 dir = monster.tracingTarget.transform.position - monster.transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            monster.transform.rotation = Quaternion.Slerp(
                monster.transform.rotation,
                targetRot,
                10f * Time.deltaTime
            );
        }
    }
}
