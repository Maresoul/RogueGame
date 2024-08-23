using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

using UnityEngine;

public class Effect_Core
{
    static Effect_Core instance = new Effect_Core();
    public static Effect_Core Instance => instance;

    /// <summary>
    /// ��ȡ�ƶ����� �ṩ��ECS����ʹ��
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public BulletMoveConfig GetBulletMoveConfig(EffectConfig entity)
    {
        BulletMoveConfig bulletMoveConfig = new BulletMoveConfig();
        //ֱ�߷���
        bulletMoveConfig.directMoveConfg_speed = entity.directMoveConfg.speed;
        bulletMoveConfig.directMoveConfg_acceleration = entity.directMoveConfg.acceleration;
        bulletMoveConfig.directMoveConfg_maxSpeed = entity.directMoveConfg.maxSpeed;
        bulletMoveConfig.directMoveConfg_custom_direction = entity.directMoveConfg.custom_direction;
        bulletMoveConfig.directMoveConfg_direction = entity.directMoveConfg.direction;
        bulletMoveConfig.directMoveConfg_space = entity.directMoveConfg.space;

        //���ٷ���
        bulletMoveConfig.trackMoveConfig_speed = entity.trackMoveConfig.speed;
        bulletMoveConfig.trackMoveConfig_torque = entity.trackMoveConfig.torque;
        bulletMoveConfig.trackMoveConfig_stopDistance = entity.trackMoveConfig.stopDistance;
        bulletMoveConfig.trackMoveConfig_x_freeze = entity.trackMoveConfig.x_freeze;
        bulletMoveConfig.trackMoveConfig_z_freeze = entity.trackMoveConfig.z_freeze;

        //Χ����ת
        bulletMoveConfig.aroundMoveConfig_speed = entity.aroundMoveConfig.speed;
        bulletMoveConfig.aroundMoveConfig_follow_speed = entity.aroundMoveConfig.follow_speed;

        //����������
        bulletMoveConfig.bezierCurveMove_duration = entity.bezierCurveMoveConfig.duration;
        bulletMoveConfig.bezierCurveMove_p1 = entity.bezierCurveMoveConfig.p1;
        bulletMoveConfig.bezierCurveMove_p2 = entity.bezierCurveMoveConfig.p2;
        bulletMoveConfig.bezierCurveMove_end = entity.bezierCurveMoveConfig.end;
        bulletMoveConfig.bezierCurveMove_type = entity.bezierCurveMoveConfig.type;

        return bulletMoveConfig;
    }

    BulletSystem bulletSystem;
    //�����ӵ��Ľӿ�
    public void DO(EffectConfig entity, ref LocalTransform player, ref EntityCommandBuffer ecb)
    {
        if (entity != null)
        {
            byte hit_type = 100;
            //��ȡ���е�����
            var move_config = GetBulletMoveConfig(entity);
            if (bulletSystem==null)
            {
                bulletSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<BulletSystem>();
            }
            //��������
            if (entity.create_type == 0)
            {
                var bullet_pos = player.Position + player.TransformDirection(entity.position_offset);
                var r = player.Rotation;//GetRotate(entity, bullet_pos, hang_point, player);

                bulletSystem.DOCreate(entity.id, entity.move_type,
                    entity.hit_range, bullet_pos, r, entity.destroy_durtaion, hit_type, ref ecb, entity.destroy_hit_count <= 1
                    , ref move_config);
            }
            //1�෢ɢ��
            else if (entity.create_type == 1)
            {
                Vector3 bullet_pos = player.Position + player.TransformDirection(entity.position_offset);
                Vector3 begin_pos = Vector3.zero;
                Quaternion begin_rotation = Quaternion.identity;
                for (int i = 0; i < entity.fan_count; i++)
                {
                    //����-����:0�ҵ㷽�� 1����Ŀ�� 2����ǰ�� 3Ŀ��ǰ��
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
                        //�������Ǵ���2�� һ������� һ�����ұ�
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
            else if (entity.create_type == 2)// 2���ζ���
            {
                Vector3 pos = player.Position;
                Vector3 offset = player.TransformDirection(entity.position_offset);
                Vector3 forward = player.Forward();
                Vector3 right = player.Right();

                //�����½ǵ�λ��
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
            else if (entity.create_type == 3)//3��Χ�����
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

    public float directMoveConfg_speed;//ֱ���ƶ�:�ƶ��ٶ�
    public float directMoveConfg_acceleration;//ֱ���ƶ�:���ٶ�/ÿ��
    public float directMoveConfg_maxSpeed;//ֱ���ƶ�:����ٶ�
    public bool directMoveConfg_custom_direction;//ֱ���ƶ�:�Ƿ��Զ����ƶ�����
    public Vector3 directMoveConfg_direction;//ֱ���ƶ�:�Զ���ķ���
    public int directMoveConfg_space;//ֱ���ƶ�:��������,0�������� 1��������

    public float trackMoveConfig_speed;//����Ŀ��_�ƶ��ٶ�
    public float trackMoveConfig_torque;//����Ŀ��_Ť��
    public float trackMoveConfig_stopDistance;//����Ŀ��_���־���
    public bool trackMoveConfig_x_freeze;//����Ŀ��_����X��
    public bool trackMoveConfig_z_freeze;//����Ŀ��_����Z��

    public float aroundMoveConfig_speed;//Χ����ת�ٶ�
    public float aroundMoveConfig_follow_speed;//Χ����ת ����Ŀ����ٶ�

    //���������ߵ�
    public float bezierCurveMove_duration;//�ƶ�ʱ��
    public float3 bezierCurveMove_begin;
    public float3 bezierCurveMove_p1;
    public float3 bezierCurveMove_p2;
    public Vector3 bezierCurveMove_end;

    public int bezierCurveMove_type;//0:���� 1:����

    public float move_elapsedTime;//�Ѿ��ƶ���ʱ��
    public bool bezierCurveMove_stop;


}