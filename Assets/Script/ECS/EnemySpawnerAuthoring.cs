using Unity.Entities;
using Unity.Mathematics;

using UnityEngine;

public class EnemySpawnerAuthoring : MonoBehaviour
{
    public GameObject enemy;//普通敌人的预制件
    public GameObject boss;//boss预制件

    public float spawnRate;//生成的频率 间隔
}

class EnemySpawnerBaker : Baker<EnemySpawnerAuthoring>
{
    public override void Bake(EnemySpawnerAuthoring authoring)
    {
        //GameObject 2 entity
        Entity entity=GetEntity(TransformUsageFlags.None);
        PrefabEntity pfefabEnity1 = new PrefabEntity()
        {   id = int.Parse(authoring.enemy.name), 
            prefab = GetEntity(authoring.enemy, TransformUsageFlags.Dynamic)
        };
        PrefabEntity pfefabEnity2 = new PrefabEntity() { id = int.Parse(authoring.boss.name), 
            prefab = GetEntity(authoring.boss, TransformUsageFlags.Dynamic) };

        AddComponent(entity, new EnemySpawnerComponent
        {
            enemy = pfefabEnity1,
            boss = pfefabEnity2,
            spawnRate = authoring.spawnRate,
            spawnPos = authoring.transform.position
        });
    }
}


public partial struct EnemySpawnerComponent : IComponentData {
    public PrefabEntity enemy;
    public PrefabEntity boss;
    public float3 spawnPos;
    public float spawnRate;
}


public struct PrefabEntity {
    public int id;
    public Entity prefab;
}