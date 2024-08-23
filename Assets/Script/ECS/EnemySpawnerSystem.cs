using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

using UnityEngine;

[BurstCompile]
public partial struct EnemySpawnerSystem:ISystem
{
    bool yet;
    Entity spawnerEntity;
    PrefabEntity enemy;
    PrefabEntity boss;
    float spawnRate;
    float3 spawnPos;
    double nextSpawnerTime;
    double nextSpawnerTime_boss;
    int global_id;

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {

        if (yet==false)
        {
            //初始化 获取要生成依赖的数据 
            if (!SystemAPI.TryGetSingletonEntity<EnemySpawnerComponent>(out spawnerEntity))
            {
                return;
            }
            RefRW<EnemySpawnerComponent> spawner = SystemAPI.GetComponentRW<EnemySpawnerComponent>(spawnerEntity);
            enemy = spawner.ValueRO.enemy;
            boss=spawner.ValueRO.boss; 

            spawnRate= spawner.ValueRO.spawnRate;
            spawnPos = spawner.ValueRO.spawnPos;
            yet = true;
        }
        else
        {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            if (nextSpawnerTime<SystemAPI.Time.ElapsedTime)
            {
                //补充生成单位的逻辑
                //GameObject.Instantiate
                for (int i = 0; i < 10; i++)
                {
                    var newEntity= ecb.Instantiate(enemy.prefab);
                    global_id += 1;

                    //给这个怪物 加一些组件 mono ai逻辑   音效  特效控制 ...
                    ecb.AddComponent(newEntity, new AgentComponent
                    {
                        unit_id = enemy.id,
                        id=global_id,
                        state=0,
                        trigger_die=0,
                    }) ;
                }
                nextSpawnerTime = SystemAPI.Time.ElapsedTime + spawnRate;
            }


            if (nextSpawnerTime_boss < SystemAPI.Time.ElapsedTime)
            {
                //补充生成单位的逻辑
                //GameObject.Instantiate
                var newEntity = ecb.Instantiate(boss.prefab);
                global_id += 1;

                //给这个怪物 加一些组件 mono ai逻辑   音效  特效控制 ...
                ecb.AddComponent(newEntity, new AgentComponent
                {
                    unit_id = boss.id,
                    id = global_id,
                    state = 0,
                    trigger_die = 0,
                });
                nextSpawnerTime_boss = SystemAPI.Time.ElapsedTime + 4;// spawnRate*5;
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}


public partial struct AgentComponent : IComponentData
{
    public int unit_id;//单位ID
    public int state;//0初始化 1移动 2死亡 3攻击
    public int id;//唯一ID
    public float trigger_die;//触发死亡的时间
}

