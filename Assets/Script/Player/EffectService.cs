using Unity.Entities;

using UnityEngine;

public class EffectService : FSMServiceBase
{

    public override void Init(FSM fsm)
    {
        base.Init(fsm);
    }

    public override void OnAnimationEnd(PlayerState state)
    {
        base.OnAnimationEnd(state);
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
    }

    public override void OnUpdate(float normalizedTime, PlayerState state)
    {
        base.OnUpdate(normalizedTime, state);
        if (state.stateEntity.effectConfigs != null && state.stateEntity.effectConfigs.Count > 0)
        {
            for (int i = 0; i < state.stateEntity.effectConfigs.Count; i++)
            {
                var e = state.stateEntity.effectConfigs[i];
                if (normalizedTime >= e.trigger && GetExcuted(i) == false)
                {
                    if (e.level == state.level)
                    {
                        SetExcuted(i);
                        DO(e, state);
                    }
                    else
                    {
                        SetExcuted(i);
                    }

                }
            }
        }
    }

    public override void ReLoop(PlayerState state)
    {
        base.ReLoop(state);
    }

    public override void ReStart(PlayerState state)
    {
        base.ReStart(state);
    }

    void DO(EffectConfig entity, PlayerState state)
    {
        if (entity != null)
        {
            byte hit_type = 0;
            if (entity.destroy_self_explosion > 0)
            {
                hit_type = 1;
            }
            var move_config = Effect_Core.Instance.GetBulletMoveConfig(entity);
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            BulletSystem bulletSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<BulletSystem>();

            if (entity.create_type == 0)
            {
                var hang_point = player.GetAtkTarget(entity.spawn_point_type, entity.spawn_hang_point);
                var bullet_pos = hang_point.transform.position + hang_point.transform.TransformDirection(entity.position_offset);
                var r = Effect_Core.Instance.GetRotate(entity, bullet_pos, hang_point, this.player);

                bulletSystem.DOCreate(entity.id, entity.move_type,
                    entity.hit_range, bullet_pos, r, entity.destroy_durtaion, hit_type, ref ecb, entity.destroy_hit_count <= 1
                    , ref move_config);

            }
            else if (entity.create_type == 1)//1多发散射
            {
                var hang_point = player.GetAtkTarget(entity.spawn_point_type, entity.spawn_hang_point);
                var bullet_pos = hang_point.transform.position + hang_point.transform.TransformDirection(entity.position_offset);
                Vector3 begin_pos = Vector3.zero;
                Quaternion begin_rotation = Quaternion.identity;
                for (int i = 0; i < entity.fan_count; i++)
                {
                    //出生-朝向:0挂点方向 1朝向目标 2自身前方 3目标前方
                    if (i == 0)
                    {
                        var r = Effect_Core.Instance.GetRotate(entity, bullet_pos, hang_point, this.player);
                        begin_pos = bullet_pos;
                        begin_rotation = r;
                        bulletSystem.DOCreate(entity.id, entity.move_type, entity.hit_range, bullet_pos, r,
                            entity.destroy_durtaion, hit_type, ref ecb, entity.destroy_hit_count <= 1
                               , ref move_config);
                    }
                    else
                    {
                        var r = Quaternion.LookRotation(begin_pos.GetOffsetPoint
                             (begin_rotation, 1, i * -entity.fan_angle_difference) - bullet_pos);
                        bulletSystem.DOCreate(entity.id, entity.move_type, entity.hit_range, bullet_pos, r,
                             entity.destroy_durtaion, hit_type, ref ecb, entity.destroy_hit_count <= 1
                             , ref move_config);

                        var r2 = Quaternion.LookRotation(begin_pos.GetOffsetPoint
                            (begin_rotation, 1, i * entity.fan_angle_difference) - bullet_pos);

                        bulletSystem.DOCreate(entity.id, entity.move_type, entity.hit_range, bullet_pos, r2,
                             entity.destroy_durtaion, hit_type, ref ecb, entity.destroy_hit_count <= 1
                                , ref move_config);
                    }
                }
            }
            else if (entity.create_type == 2)// 2矩形队列
            {
                var hang_point = player.GetAtkTarget(entity.spawn_point_type, entity.spawn_hang_point);
                Vector3 forward = hang_point.transform.forward;
                Vector3 right = hang_point.transform.right;
                Vector3 spawn_point = hang_point.transform.position -
                    entity.rect_columns / 2.0f * right * entity.rect_columns_spacing
                    + right * (entity.rect_columns_spacing / 2.0f) +
                    hang_point.transform.TransformDirection(entity.position_offset);

                for (int i = 0; i < entity.rect_rows; i++)
                {
                    for (int j = 0; j < entity.rect_columns; j++)
                    {
                        //skill_obj.transform.position = hang_point.transform.position;
                        var bullet_pos = spawn_point +
                            forward * i * entity.rect_rows_spacing
                            + right * j * entity.rect_columns_spacing;

                        var r = Effect_Core.Instance.GetRotate(entity, bullet_pos, hang_point, this.player);
                        bulletSystem.DOCreate(entity.id, entity.move_type, entity.hit_range, bullet_pos, r,
                             entity.destroy_durtaion, hit_type, ref ecb, entity.destroy_hit_count <= 1
                             , ref move_config);
                    }
                }

            }
            else if (entity.create_type == 3)//3范围内随机
            {
                var hang_point = player.GetAtkTarget(entity.spawn_point_type, entity.spawn_hang_point);
                Vector3 spawn_point = hang_point.transform.position
                    + hang_point.transform.TransformDirection(entity.position_offset);

                var rotation = hang_point.transform.rotation;
                for (int i = 0; i < entity.random_count; i++)
                {
                    var bullet_pos = spawn_point.GetOffsetPoint
                        (rotation, IntEx.Range(entity.random_radius, entity.random_radius_max),
                        IntEx.Range(entity.random_angle, entity.random_angle_max));
                    var r = Effect_Core.Instance.GetRotate(entity, bullet_pos, hang_point, this.player);
                    bulletSystem.DOCreate(entity.id, entity.move_type, entity.hit_range, bullet_pos, r,
                         entity.destroy_durtaion, hit_type, ref ecb, entity.destroy_hit_count <= 1
                         , ref move_config);
                }
            }

            ecb.Playback(World.DefaultGameObjectInjectionWorld.EntityManager);
            ecb.Dispose();
            return;
        }
    }

}
