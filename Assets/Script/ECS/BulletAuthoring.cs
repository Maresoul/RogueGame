using Unity.Entities;

using UnityEngine;

public class BulletAuthoring : MonoBehaviour
{
    [Header("ը��")]
    public GameObject Bomb01;
    [Header("����")]
    public GameObject Dart01;
    [Header("�ɵ�")]
    public GameObject Knife01;
    [Header("����")]
    public GameObject RoundWheel01;

    [Header("�ӵ�_ǹ��")]
    public GameObject B1051;

    [Header("�ӵ�_����")]
    public GameObject B1052;


    [Header("ը��_�Ա���Ч")]
    public GameObject Bomb01_explosion;

    [Header("���ڡ��ɵ�������������Ч")]
    public GameObject Blood;


    [Header("������")]
    public GameObject Drop;

    [Header("�����ӵ�")]
    public GameObject Enemy_Bullet;
}


class BulletBaker : Baker<BulletAuthoring>
{
    public override void Bake(BulletAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new BulletRes()
        {
            Bomb01 = GetEntity(authoring.Bomb01, TransformUsageFlags.Dynamic),
            Dart01 = GetEntity(authoring.Dart01, TransformUsageFlags.Dynamic),
            Knife01 = GetEntity(authoring.Knife01, TransformUsageFlags.Dynamic),
            RoundWheel01 = GetEntity(authoring.RoundWheel01, TransformUsageFlags.Dynamic),
            B1051 = GetEntity(authoring.B1051, TransformUsageFlags.Dynamic),
            B1052 = GetEntity(authoring.B1052, TransformUsageFlags.Dynamic),
            Bomb01_explosion = GetEntity(authoring.Bomb01_explosion, TransformUsageFlags.Dynamic),
            Blood = GetEntity(authoring.Blood, TransformUsageFlags.Dynamic),
            Drop = GetEntity(authoring.Drop, TransformUsageFlags.Dynamic),
            Enemy_Bullet = GetEntity(authoring.Enemy_Bullet, TransformUsageFlags.Dynamic),
        });
    }
}


public partial struct BulletRes : IComponentData
{
    public Entity Bomb01;
    public Entity Dart01;
    public Entity Knife01;
    public Entity RoundWheel01;

    public Entity B1051;
    public Entity B1052;

    public Entity Bomb01_explosion;
    public Entity Blood;

    public Entity Drop;

    public Entity Enemy_Bullet;

}