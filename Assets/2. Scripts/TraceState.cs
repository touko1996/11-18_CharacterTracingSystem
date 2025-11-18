using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraceState : IState
{
    private Monster monster;

    public TraceState(Monster monster)
    {
        this.monster = monster;
    }

    public void Enter()
    {
        monster.anim.SetFloat("Blend", 1f);
    }
    public void Update()
    {
        Vector3 dir = monster.tracingTarget.transform.position - monster.transform.position;
        dir.y = 0f;

        Quaternion targetRot = Quaternion.LookRotation(dir);
        monster.transform.rotation = Quaternion.Slerp(monster.transform.rotation, targetRot, 10f*Time.deltaTime);

        if (monster.canAttackPlayer)
        {
            monster.ChangeState(new AttackState(monster)); return; //더이상 업데이트안해!
        }
        
        monster.transform.position =
        Vector3.MoveTowards(monster.transform.position,monster.tracingTarget.transform.position,monster.moveSpeed * Time.deltaTime);

        if (!monster.canTracePlayer) 
        {
            monster.ChangeState(new ReturnState(monster)); 
        }
        
    }
    public void Exit() { }
    
}
