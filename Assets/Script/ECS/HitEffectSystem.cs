using System.Collections;
using System.Collections.Generic;

using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class HitEffectSystem : SystemBase
{

    public Dictionary<int, float3> Blood = new Dictionary<int, float3>();
    public List<float3> Explosion = new List<float3>();

    public Entity hit_effect;
    public Entity explosion_effect;
    public List<Entity> wait_destroy = new List<Entity>();
    float3 offset = new float3(0, 0.5f, 0);
    public void SetHitEffect(Entity hit_effect, Entity explosion_effect)
    {
        this.hit_effect = hit_effect;
        this.explosion_effect = explosion_effect;
    }

    //在指定位置 生成一个爆炸的特效
    public void AddExplosion(float3 pos)
    {
        Explosion.Add(pos);
    }

    public void Add(int id, float3 pos)
    {
        Blood[id] = pos;
    }

    protected override void OnUpdate()
    {
        if (Blood.Count > 0)
        {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var item in Blood)//主角掉血
            {
                var t = ecb.Instantiate(hit_effect);//创建特效

                ecb.AddComponent(t, new HitEffectComponent
                {
                    state = 0,
                    create_time = SystemAPI.Time.ElapsedTime,
                    pos = item.Value,
                    type = 0,
                });
            }
            Blood.Clear();
            ecb.Playback(World.EntityManager);
            ecb.Dispose();
        }

        if (Explosion.Count > 0)
        {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var item in Explosion)
            {
                var t = ecb.Instantiate(explosion_effect);
                ecb.AddComponent(t, new HitEffectComponent
                {
                    state = 0,
                    create_time = SystemAPI.Time.ElapsedTime,
                    pos = item,
                    type = 1,
                });
            }
            Explosion.Clear();
            ecb.Playback(World.EntityManager);
            ecb.Dispose();
        }

        var entityManager = World.EntityManager;
        NativeArray<Entity> entities = entityManager.GetAllEntities(Allocator.Temp);

        foreach (var item in entities)
        {
            if (World.EntityManager.HasComponent<HitEffectComponent>(item))
            {
                var hiteffect = World.EntityManager.GetComponentData<HitEffectComponent>(item);
                if (hiteffect.state == 0)
                {
                    hiteffect.state = 1;
                    //设置位置
                    var ls = World.EntityManager.GetComponentData<LocalTransform>(item);
                    ls.Position = hiteffect.pos;
                    World.EntityManager.SetComponentData(item, ls);
                    World.EntityManager.SetComponentData(item, hiteffect);
                }
                else if (hiteffect.state == 1)
                {
                    float destroy_time = 1;
                    if (hiteffect.type == 0)
                    {
                        destroy_time = 0.5f;
                    }
                    if (SystemAPI.Time.ElapsedTime - hiteffect.create_time >= destroy_time)
                    {
                        hiteffect.state = 2;
                        World.EntityManager.SetComponentData(item, hiteffect);
                        //回收
                        wait_destroy.Add(item);
                    }
                }
            }
        }

        if (wait_destroy.Count > 0)
        {
            foreach (var item in wait_destroy)
            {
                World.EntityManager.DestroyEntity(item);
            }
            wait_destroy.Clear();
        }
    }
}

//挂载一个
public struct HitEffectComponent : IComponentData
{
    public int state;//状态0出生 1设置好位置 2销毁
    public double create_time;//创建时间
    public float3 pos;
    public byte type;//类型0持续检测 1自爆
}