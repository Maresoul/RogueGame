using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

using UnityEngine;

[BurstCompile]
public struct EnemyJob : IJob
{
    //jobsystem �ܶ�����   

    float3 p_pos;//���ǵ�λ��
    float3 agent_pos;//�����λ�� 
    float3 s_pos;//��λ��λ��
    float speed;//�ٶ�

    private NativeArray<byte> _rot_update;//�Ƿ������ת 1��ʾ��Ҫ������ת 0����Ҫ
    private NativeArray<quaternion> _rot;//ע������quaternion
    private NativeArray<float3> _prefVelocity;//�洢���ŵ��ٶ�

    public EnemyJob(float3 p_pos, float3 a_pos, float3 s_pos, float speed, NativeArray<byte> _rot_update,
         NativeArray<quaternion> _rot, NativeArray<float3> _prefVelocity)
    {
        this.p_pos = p_pos;
        this.agent_pos = a_pos;
        this.s_pos = s_pos;
        this.speed = speed;

        this._rot_update = _rot_update;
        this._rot = _rot;
        this._prefVelocity = _prefVelocity;
    }

    //�����jobִ�е�ʱ��,ʵ�ʾ��ǵ�������ӿ�
    public void Execute()
    {
        var r1 = agent_pos - s_pos;
        if (r1[0] != 0 || r1[1] != 0 || r1[2] != 0)
        {
            var r = quaternion.LookRotation(r1, Vector3.up);
            _rot[0] = r;
            _rot_update[0] = 1;
        }
        else
        {
            _rot_update[0] = 0;
        }

        float s = speed * Unity.Mathematics.math.min(1f, (Unity.Mathematics.math.distance(agent_pos, p_pos) / speed));
        _prefVelocity[0] = Unity.Mathematics.math.normalize(p_pos - agent_pos) * s;

    }
}
