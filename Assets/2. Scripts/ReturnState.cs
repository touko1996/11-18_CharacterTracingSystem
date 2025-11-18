using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnState : IState
{
    private Monster monster;

    public ReturnState(Monster monster)
    {
        this.monster = monster;
    }

    public void Enter()
    {
        monster.anim.SetFloat("Blend", 1f);
    }

    public void Update()
    {
        // 1) 집(homePosition) 방향으로 회전
        Vector3 dir = monster.homePosition - monster.transform.position;
        dir.y = 0f;

        if (dir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            monster.transform.rotation =
                Quaternion.Slerp(monster.transform.rotation, targetRot, 10f * Time.deltaTime);
        }

        // 2) homePosition으로 이동
        monster.transform.position =
            Vector3.MoveTowards(
                monster.transform.position,
                monster.homePosition,
                monster.moveSpeed * Time.deltaTime);

        // 3) homePosition 도착 시 → PatrolState로 전환
        if (monster.isHomePoint)
        {
            monster.ChangeState(new PatrolState(monster));
        }
    }


    public void Exit() { }
}
