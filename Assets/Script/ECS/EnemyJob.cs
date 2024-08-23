using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

using UnityEngine;

[BurstCompile]
public struct EnemyJob : IJob
{
    //jobsystem 很多任务   

    float3 p_pos;//主角的位置
    float3 agent_pos;//代理的位置 
    float3 s_pos;//单位的位置
    float speed;//速度

    private NativeArray<byte> _rot_update;//是否更新旋转 1表示需要更新旋转 0不需要
    private NativeArray<quaternion> _rot;//注视主角quaternion
    private NativeArray<float3> _prefVelocity;//存储最优的速度

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

    //当这个job执行的时候,实际就是调度这个接口
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
