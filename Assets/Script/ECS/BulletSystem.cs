using System.Collections.Generic;

using GPUECSAnimationBaker.Engine.AnimatorSystem;

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

using UnityEngine;

public partial class BulletSystem : SystemBase
{
    EnemySystem es;
    HitEffectSystem hitEffectSystem;
    Transform player;
    DropSystem drop_system;
    List<Entity> wait_destroy_bullet = new List<Entity>();

    protected override void OnUpdate()
    {
        if (yet == false) {
            return;
        }
        var entityManager = World.EntityManager;
        NativeArray<Entity> entities = entityManager.GetAllEntities(Allocator.Temp);

        //负责对子弹状态的管理 命中检测 飞行....
        foreach (var target in entities)
        {
            if(World.EntityManager.HasComponent<BulletEntity>(target))
            {
                BulletEntity bulletEntity = World.EntityManager.GetComponentData<BulletEntity>(target);
                if (bulletEntity.state == 0)
                {
                    LocalTransform LT = World.EntityManager.GetComponentData<LocalTransform>(target);
                    LT.Position = bulletEntity.init_position;
                    LT.Rotation = bulletEntity.init_rotation;
                    bulletEntity.InitPos(ref LT);
                    World.EntityManager.SetComponentData(target, LT);

                    //Debug.LogError("设置位置:" + LT.Position);
                    bulletEntity.state = 1;
                    World.EntityManager.SetComponentData(target, bulletEntity);
                }
                else if (bulletEntity.state==1)
                {
                    LocalTransform LT = World.EntityManager.GetComponentData<LocalTransform>(target);

                    if (bulletEntity.move_type == 0)
                    {
                        //移动类型:0按方向移动 1追踪移动 2围绕旋转 3贝塞尔曲线移动
                        DOMove_ByDirection(ref LT, ref bulletEntity);// bulletEntity.move_speed);
                    }
                    else if (bulletEntity.move_type == 1)
                    {
                        DOMove_Track(ref LT, ref bulletEntity);
                    }
                    else if (bulletEntity.move_type == 2)
                    {
                        Move_Around(ref LT, ref bulletEntity);
                    }
                    else if (bulletEntity.move_type == 3)
                    {
                        Move_BezierCurve(ref LT, ref bulletEntity);
                        World.EntityManager.SetComponentData(target, bulletEntity);
                    }
                    World.EntityManager.SetComponentData(target, LT);

                    //命中检测(全局的单位 做一个管理 分配到不同的格子上)
                    //每一次进行命中检测,都需要对所有单位与子弹进行距离的计算 一万个单位 命中的范围附近的格子 10个单位 
                    if (bulletEntity.hit_type == 0)
                    {
                        HitTest(ref LT, bulletEntity.hit_range, es, bulletEntity.id, bulletEntity.on_hit_destroy);
                    }
                    else if (bulletEntity.hit_type == 1)
                    {
                        if (OutTime(bulletEntity))
                        {
                            HitTest(ref LT, bulletEntity.hit_range, es, bulletEntity.id, false);
                            //创建爆炸的特效
                            hitEffectSystem.AddExplosion(LT.Position);
                            //销毁
                            wait_destroy_bullet.Add(target);
                        }
                    }
                    else if (bulletEntity.hit_type == 100)//表示敌人创建的子弹,需要监测是否命中主角
                    {
                        Vector3 p_pos = es.player.transform.position;
                        //站位判定:攻击方与受击方
                        if (math.abs(LT.Position.x - p_pos.x) <= 1.5f && math.abs(LT.Position.z - p_pos.z) <= 1.5f)
                        {
                            //有需要加上角度判定
                            es.p_fsm.BeHit(LT.Position, LT.Forward());
                            wait_destroy_bullet.Add(target);
                            //创建掉血特效
                            hitEffectSystem.Add(1001, es.player.transform.position + new Vector3(0, 1.5f, 0));
                        }
                    }

                    if (bulletEntity.hit_type != 1 && OutTime(bulletEntity))
                    {
                        //销毁
                        wait_destroy_bullet.Add(target);
                    }
                }

            }
        }

        if (wait_destroy_bullet.Count > 0)
        {
            foreach (var item in wait_destroy_bullet)
            {
                World.EntityManager.DestroyEntity(item);
            }
            wait_destroy_bullet.Clear();
        }
    }


    //超时的判断
    bool OutTime(BulletEntity e)
    {
        return SystemAPI.Time.ElapsedTime - e.spawn_time >= e.duration;
    }


    void HitTest(ref LocalTransform lt, float hit_range, EnemySystem es, int bulletId, bool on_hit_destroy)
    {
        WorldUnitManager.Instance.OnHit(lt.Position, hit_range, World.EntityManager, es, OnHit, bulletId, on_hit_destroy);
    }

    //Dictionary<int,>
    void OnHit(EntityManager manage, EnemySystem es, AIEntity item, float3 bullet_pos, float range, int bulletId)
    {
        if (item.ac.state != 2)
        {
            if (math.abs(bullet_pos.x - item.ls.Position.x) <= range && math.abs(bullet_pos.z - item.ls.Position.z) <= range)
            {
                es.Add_WaitDestroy(item.e);//需要让这个敌人死亡
                item.ac.state = 2;//状态设置为2 死亡,需要等待移除
                item.ac.trigger_die = UnityEngine.Time.time;
                manage.SetComponentData(item.e, item.ac);

                var xx = es.GetEntity(item.ac.id);
                if (xx != null)
                {
                    //播放死亡的动作
                    xx.Play(ref manage, AnimationIdsAnmState1001.die.GetHashCode());

                    //掉落
                    var r = UnityEngine.Random.Range(0, 100);
                    if (r >= 90)
                    {
                        var drop = EntityManager.Instantiate(res.Drop);
                        var lt = EntityManager.GetComponentData<LocalTransform>(drop);
                        lt.Position = item.ls.Position + new float3(0, 1.5f, 0);
                        EntityManager.SetComponentData<LocalTransform>(drop, lt);
                        drop_system.Add(drop);
                        //EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
                        //ecb.Instantiate(res.Drop);
                    }
                }
                //hitEffectSystem.Add(item.ac.id, manage.GetComponentData<LocalTransform>( item.e).Position);
            }
        }
    }

    public Entity GetCreateTarget(int id)
    {
        if (id == 100)
        {
            return res.Bomb01;
        }
        else if (id == 101)
        {
            return res.Dart01;
        }
        else if (id == 102)
        {
            return res.Knife01;
        }
        else if (id == 103)
        {
            return res.RoundWheel01;
        }
        else if (id == 1051)
        {
            return res.B1051;
        }
        else if (id == 1052)
        {
            return res.B1052;
        }
        else if (id == 201)
        {
            return res.Enemy_Bullet;
        }
        else
        {
            return res.Bomb01;
        }
    }


    bool yet;
    Entity resEntity;
    BulletRes res;
    //单个的接口
    public void DOCreate(int id, byte move_type, float hit_range,
        float3 init_position, quaternion init_rotation, float duration, byte hit_type,
        ref EntityCommandBuffer ecb, bool on_hit_destroy, ref BulletMoveConfig bulletMoveConfig)
    {

        if (yet == false)
        {
            //BulletRes
            if (!SystemAPI.TryGetSingletonEntity<BulletRes>(out resEntity))
            {
                return;
            }

            if (es == null)
            {
                es = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<EnemySystem>();
                player = GameObject.Find("1001").transform;
                drop_system = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<DropSystem>();

                res = World.EntityManager.GetComponentData<BulletRes>(resEntity);

                hitEffectSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<HitEffectSystem>();
                hitEffectSystem.SetHitEffect(res.Blood, res.Bomb01_explosion);
            }

            //hitEffectSystem = World.GetOrCreateSystemManaged<HitEffectSystem>();
            //hitEffectSystem.SetHitEffect(res.Blood, res.Bomb01_explosion);

            bulletMoveConfig.bezierCurveMove_begin = init_position;


            yet = true;
        }

        var target = ecb.Instantiate(GetCreateTarget(id));

        ecb.AddComponent(target, new BulletEntity()
        {
            state = 0,
            move_type = move_type,
            hit_range = hit_range,
            init_position = init_position,
            init_rotation = init_rotation,
            spawn_time = SystemAPI.Time.ElapsedTime,
            duration = duration,
            hit_type = hit_type,
            on_hit_destroy = on_hit_destroy,
            target_id = 0,
            bulletMoveConfig = bulletMoveConfig,
        });
    }


    public float3 GetTarget_Position(ref BulletEntity bulletEntity)
    {
        if (bulletEntity.target_id == 0)
        {
            return player.position;//飞向主角
        }
        else
        {
            return es.GetEntity(bulletEntity.target_id).ls.Position;//朝某个敌人飞行
        }
    }


    //0按方向移动 1追踪移动 2围绕旋转 3贝塞尔曲线移动

    void DOMove_ByDirection(ref LocalTransform lt, ref BulletEntity bulletEntity)// float move_speed)
    {
        lt.Position = lt.Position + lt.Forward() * bulletEntity.bulletMoveConfig.directMoveConfg_speed * UnityEngine.Time.deltaTime;
    }


    void DOMove_Track(ref LocalTransform lt, ref BulletEntity bulletEntity)
    {
        //先找到目标,然后朝向目标
        //然后再向自己前方进行移动
        float3 target_pos = GetTarget_Position(ref bulletEntity);
        lt.Rotation = Quaternion.Lerp(lt.Rotation, Quaternion.LookRotation(target_pos, new float3(0, 1, 0)), bulletEntity.bulletMoveConfig.trackMoveConfig_torque * UnityEngine.Time.deltaTime); ;
        lt.Position = lt.Position + lt.Forward() * bulletEntity.bulletMoveConfig.trackMoveConfig_speed * UnityEngine.Time.deltaTime;
    }


    void Move_Around(ref LocalTransform lt, ref BulletEntity bulletEntity)
    {
        // 计算旋转量
        float angle = bulletEntity.bulletMoveConfig.aroundMoveConfig_speed * UnityEngine.Time.deltaTime;
        quaternion deltaRotation = quaternion.AxisAngle(new float3(0, 1, 0), angle);

        // 应用旋转
        lt.Rotation = math.mul(deltaRotation, lt.Rotation);

        // 计算新的位置
        float3 originalPosition = lt.Position;
        float3 target_pos = GetTarget_Position(ref bulletEntity);
        float3 newPosition = target_pos + math.mul(deltaRotation, (originalPosition - target_pos));

        // 更新位置
        lt.Position = newPosition;
    }


    void Move_BezierCurve(ref LocalTransform lt, ref BulletEntity bulletEntity)
    {
        if (bulletEntity.bulletMoveConfig.bezierCurveMove_stop == false)
        {
            //已经飞行的时长
            bulletEntity.bulletMoveConfig.move_elapsedTime += UnityEngine.Time.deltaTime;
            //已经飞行的时长/总的飞行时长 
            float t = Mathf.Clamp01(bulletEntity.bulletMoveConfig.move_elapsedTime / bulletEntity.bulletMoveConfig.bezierCurveMove_duration);
            if (bulletEntity.bulletMoveConfig.bezierCurveMove_type == 0)
            {
                lt.Position = BezierCurve.GetPointOnQuadraticBezierCurve(bulletEntity.bulletMoveConfig.bezierCurveMove_begin,
                 bulletEntity.bulletMoveConfig.bezierCurveMove_p1, bulletEntity.bulletMoveConfig.bezierCurveMove_end, t);
            }
            else if (bulletEntity.bulletMoveConfig.bezierCurveMove_type == 1)
            {
                lt.Position = BezierCurve.GetPointOnCubicBezierCurve(bulletEntity.bulletMoveConfig.bezierCurveMove_begin,
                bulletEntity.bulletMoveConfig.bezierCurveMove_p1, bulletEntity.bulletMoveConfig.bezierCurveMove_p2,
                bulletEntity.bulletMoveConfig.bezierCurveMove_end, t);
            }
            if (t >= 1)
            {
                bulletEntity.bulletMoveConfig.bezierCurveMove_stop = true;
            }
        }
    }
}




public struct BulletEntity : IComponentData
{
    public int id;
    public byte state;//0出生(初始化位置和角度) 1生效(运动) 2失效 
    public byte move_type;//运动类型:0按方向移动 1追踪移动 2围绕旋转 3贝塞尔曲线
    public float hit_range;//命中范围
    public float3 init_position;
    public quaternion init_rotation;
    public double spawn_time;//出生时间
    public float duration;//存活时间
    public byte hit_type;//伤害类型:0每帧检测 1自爆范围检测
    public bool on_hit_destroy;
    public int target_id;//目标ID
    public BulletMoveConfig bulletMoveConfig;
    public BulletEntity(int id, byte state, byte move_type,
        float hit_range, float3 init_position, quaternion init_rotation,
        double spawn_time, float duration, byte hit_type, bool on_hit_destroy, int target_id, BulletMoveConfig bulletMoveConfig)
    {
        this.id = id;
        this.state = state;
        this.move_type = move_type;
        this.hit_range = hit_range;
        this.init_position = init_position;
        this.init_rotation = init_rotation;
        this.spawn_time = spawn_time;
        this.duration = duration;
        this.hit_type = hit_type;
        this.on_hit_destroy = on_hit_destroy;
        this.target_id = target_id;
        this.bulletMoveConfig = bulletMoveConfig;
    }
    //为贝塞尔曲线服务的 需要提前计算 
    public void InitPos(ref LocalTransform lt)
    {
        if (move_type == 3)
        {
            bulletMoveConfig.bezierCurveMove_begin = init_position;
            //根据这个特效的位置 求得操作点的位置
            bulletMoveConfig.bezierCurveMove_p1 = init_position + lt.TransformDirection(bulletMoveConfig.bezierCurveMove_p1);

            if (bulletMoveConfig.bezierCurveMove_type == 1)
            {
                bulletMoveConfig.bezierCurveMove_p2 = init_position + lt.TransformDirection(bulletMoveConfig.bezierCurveMove_p2);
            }
            //结束点
            bulletMoveConfig.bezierCurveMove_end = init_position + lt.TransformDirection(bulletMoveConfig.bezierCurveMove_end);
        }
    }
}

