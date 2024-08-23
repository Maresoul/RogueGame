using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

using UnityEngine;

public class Effect_Core
{
    static Effect_Core instance = new Effect_Core();
    public static Effect_Core Instance => instance;

    /// <summary>
    /// 获取移动参数 提供给ECS进行使用
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public BulletMoveConfig GetBulletMoveConfig(EffectConfig entity)
    {
        BulletMoveConfig bulletMoveConfig = new BulletMoveConfig();
        //直线飞行
        bulletMoveConfig.directMoveConfg_speed = entity.directMoveConfg.speed;
        bulletMoveConfig.directMoveConfg_acceleration = entity.directMoveConfg.acceleration;
        bulletMoveConfig.directMoveConfg_maxSpeed = entity.directMoveConfg.maxSpeed;
        bulletMoveConfig.directMoveConfg_custom_direction = entity.directMoveConfg.custom_direction;
        bulletMoveConfig.directMoveConfg_direction = entity.directMoveConfg.direction;
        bulletMoveConfig.directMoveConfg_space = entity.directMoveConfg.space;

        //跟踪飞行
        bulletMoveConfig.trackMoveConfig_speed = entity.trackMoveConfig.speed;
        bulletMoveConfig.trackMoveConfig_torque = entity.trackMoveConfig.torque;
        bulletMoveConfig.trackMoveConfig_stopDistance = entity.trackMoveConfig.stopDistance;
        bulletMoveConfig.trackMoveConfig_x_freeze = entity.trackMoveConfig.x_freeze;
        bulletMoveConfig.trackMoveConfig_z_freeze = entity.trackMoveConfig.z_freeze;

        //围绕旋转
        bulletMoveConfig.aroundMoveConfig_speed = entity.aroundMoveConfig.speed;
        bulletMoveConfig.aroundMoveConfig_follow_speed = entity.aroundMoveConfig.follow_speed;

        //贝塞尔曲线
        bulletMoveConfig.bezierCurveMove_duration = entity.bezierCurveMoveConfig.duration;
        bulletMoveConfig.bezierCurveMove_p1 = entity.bezierCurveMoveConfig.p1;
        bulletMoveConfig.bezierCurveMove_p2 = entity.bezierCurveMoveConfig.p2;
        bulletMoveConfig.bezierCurveMove_end = entity.bezierCurveMoveConfig.end;
        bulletMoveConfig.bezierCurveMove_type = entity.bezierCurveMoveConfig.type;

        return bulletMoveConfig;
    }

    BulletSystem bulletSystem;
    //创建子弹的接口
    public void DO(EffectConfig entity, ref LocalTransform player, ref EntityCommandBuffer ecb)
    {
        if (entity != null)
        {
            byte hit_type = 100;
            //获取飞行的配置
            var move_config = GetBulletMoveConfig(entity);
            if (bulletSystem==null)
            {
                bulletSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<BulletSystem>();
            }
            //创建单个
            if (entity.create_type == 0)
            {
                var bullet_pos = player.Position + player.TransformDirection(entity.position_offset);
                var r = player.Rotation;//GetRotate(entity, bullet_pos, hang_point, player);

                bulletSystem.DOCreate(entity.id, entity.move_type,
                    entity.hit_range, bullet_pos, r, entity.destroy_durtaion, hit_type, ref ecb, entity.destroy_hit_count <= 1
                    , ref move_config);
            }
            //1多发散射
            else if (entity.create_type == 1)
            {
                Vector3 bullet_pos = player.Position + player.TransformDirection(entity.position_offset);
                Vector3 begin_pos = Vector3.zero;
                Quaternion begin_rotation = Quaternion.identity;
                for (int i = 0; i < entity.fan_count; i++)
                {
                    //出生-朝向:0挂点方向 1朝向目标 2自身前方 3目标前方
                    if (i == 0)
                    {
                        var r = player.Rotation;// GetRotate(entity, bullet_pos, hang_point, player);
                        begin_pos = bullet_pos;
                        begin_rotation = r;
                        bulletSystem.DOCreate(entity.id, entity.move_type, entity.hit_range, bullet_pos, r,
                            entity.destroy_durtaion, hit_type, ref ecb, entity.destroy_hit_count <= 1
                               , ref move_config);
                    }
                    else
                    {
                        //其他都是创建2个 一个在左边 一个在右边
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
                Vector3 pos = player.Position;
                Vector3 offset = player.TransformDirection(entity.position_offset);
                Vector3 forward = player.Forward();
                Vector3 right = player.Right();

                //求左下角的位置
                Vector3 spawn_point = pos -
                    entity.rect_columns / 2.0f * right * entity.rect_columns_spacing
                    + right * (entity.rect_columns_spacing / 2.0f) + offset;

                for (int i = 0; i < entity.rect_rows; i++)
                {
                    for (int j = 0; j < entity.rect_columns; j++)
                    {
                        //skill_obj.transform.position = hang_point.transform.position;
                        var bullet_pos = spawn_point +
                            forward * i * entity.rect_rows_spacing
                            + right * j * entity.rect_columns_spacing;

                        var r = player.Rotation;//GetRotate(entity, bullet_pos, hang_point, player);

                        bulletSystem.DOCreate(entity.id, entity.move_type, entity.hit_range, bullet_pos, r,
                             entity.destroy_durtaion, hit_type, ref ecb, entity.destroy_hit_count <= 1
                             , ref move_config);
                    }
                }

            }
            else if (entity.create_type == 3)//3范围内随机
            {
                Vector3 hang_point = player.Position;
                Vector3 offset = player.TransformDirection(entity.position_offset);
                Vector3 spawn_point = hang_point + offset;

                var rotation = player.Rotation;
                for (int i = 0; i < entity.random_count; i++)
                {
                    var bullet_pos = spawn_point.GetOffsetPoint
                        (rotation, IntEx.Range(entity.random_radius, entity.random_radius_max),
                        IntEx.Range(entity.random_angle, entity.random_angle_max));
                    var r = player.Rotation;// GetRotate(entity, bullet_pos, hang_point, player);
                    bulletSystem.DOCreate(entity.id, entity.move_type, entity.hit_range, bullet_pos, r,
                         entity.destroy_durtaion, hit_type, ref ecb, entity.destroy_hit_count <= 1
                         , ref move_config);
                }
            }
        }
    }

    public Quaternion GetRotate(EffectConfig entity, Vector3 bullet_pos, GameObject hang_point, FSM player)
    {
        if (entity.rotate_type == 0)
        {
            return Quaternion.LookRotation(hang_point.transform.forward);
        }
        //else if (entity.rotate_type == 1)
        //{
        //    return Quaternion.LookRotation(player.atk_fsm._transform.position - bullet_pos);
        //}
        else if (entity.rotate_type == 2)
        {
            return Quaternion.LookRotation(player._transform.forward);
        }
        //else if (entity.rotate_type == 3)
        //{
        //    return Quaternion.LookRotation(player.atk_fsm._transform.forward);
        //}
        else
        {
            return Quaternion.identity;
        }
    }

}


public struct BulletMoveConfig
{

    public float directMoveConfg_speed;//直线移动:移动速度
    public float directMoveConfg_acceleration;//直线移动:加速度/每秒
    public float directMoveConfg_maxSpeed;//直线移动:最大速度
    public bool directMoveConfg_custom_direction;//直线移动:是否自定义移动方向
    public Vector3 directMoveConfg_direction;//直线移动:自定义的方向
    public int directMoveConfg_space;//直线移动:坐标类型,0世界坐标 1本地坐标

    public float trackMoveConfig_speed;//跟踪目标_移动速度
    public float trackMoveConfig_torque;//跟踪目标_扭矩
    public float trackMoveConfig_stopDistance;//跟踪目标_保持距离
    public bool trackMoveConfig_x_freeze;//跟踪目标_冻结X轴
    public bool trackMoveConfig_z_freeze;//跟踪目标_冻结Z轴

    public float aroundMoveConfig_speed;//围绕旋转速度
    public float aroundMoveConfig_follow_speed;//围绕旋转 跟随目标的速度

    //贝塞尔曲线的
    public float bezierCurveMove_duration;//移动时长
    public float3 bezierCurveMove_begin;
    public float3 bezierCurveMove_p1;
    public float3 bezierCurveMove_p2;
    public Vector3 bezierCurveMove_end;

    public int bezierCurveMove_type;//0:二阶 1:三阶

    public float move_elapsedTime;//已经移动的时长
    public bool bezierCurveMove_stop;


}