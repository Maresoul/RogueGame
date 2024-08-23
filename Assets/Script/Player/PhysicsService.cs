using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PhysicsService : FSMServiceBase
{
    public override void Init(FSM fsm)
    {
        base.Init(fsm);
    }

    public override void OnAnimationEnd(PlayerState state)
    {
        base.OnAnimationEnd(state);
        ReSetAllExcuted();
        Stop();
    }

    public override void OnBegin(PlayerState state)
    {
        base.OnBegin(state);
        ReSetAllExcuted();
    }

    public override void OnDisable(PlayerState state)
    {
        base.OnDisable(state);
    }

    public override void OnEnd(PlayerState state)
    {
        base.OnEnd(state);
        ReSetAllExcuted();
        Stop();
    }

    public override void OnUpdate(float normalizedTime, PlayerState state)
    {
        base.OnUpdate(normalizedTime, state);
        var e = state.stateEntity.physicsConfig;
        if (e != null && e.Count > 0)
        {
            for (int i = 0; i < e.Count; i++)
            {
                var entity = e[i];
                if (normalizedTime >= entity.trigger && GetExcuted(i) == false)
                {
                    Do(entity, state);
                    SetExcuted(i);
                }
            }
        }

        if (begin)
        {
            //动作进度 小于 配置的结束点
            if (normalizedTime <= currentEntity.time)
            {
                if (currentEntity.time > 0)
                {
                    //已经执行的时间 / 需要执行的时间
                    var f = (normalizedTime - currentEntity.trigger) / (currentEntity.time - currentEntity.trigger);
                    float lerpTime = currentEntity.cure.Evaluate(f);
                    var speed = Vector3.Lerp(Vector3.zero, force, lerpTime);

                    player.AddForce(speed, currentEntity.ignore_gravity);

                    //if (currentEntity.stop_dst > 0)
                    //{
                    //    var begin = player._transform.position + Vector3.up;
                    //    var result = Physics.Linecast(begin, begin + player._transform.forward * currentEntity.stop_dst, player.GetEnemyLayerMask());
                    //    if (result)
                    //    {
                    //        Stop();
                    //    }
                    //}
                }
            }
            else
            {
                Stop();
            }
        }
    }

    private void Stop()
    {
        if (begin)
        {
            begin = false;
            player.RemoveForce();
        }
    }

    bool begin = false;
    PhysicsConfig currentEntity;
    Vector3 force;
    public void Do(PhysicsConfig entity, PlayerState state)
    {
        //执行这个配置所需要花费的时间
        float t = state.clipLength * ((entity.time - entity.trigger) / 1);
        if (t <= 0)
        {
            begin = false;
        }
        else
        {
            currentEntity = entity;
            if (entity.time > 0)
            {
                force = currentEntity.force / t;
            }
            else
            {
                force = currentEntity.force;

            }
            //force *= player.GetForceMutilpe();
            begin = true;
        }
    }

    public override void ReLoop(PlayerState state)
    {
        base.ReLoop(state);
    }

    public override void ReStart(PlayerState state)
    {
        base.ReStart(state);
        ReSetAllExcuted();
    }
}
