using System;
using System.Collections.Generic;

using Game.Config;

using GPUECSAnimationBaker.Engine.AnimatorSystem;
using Nebukam.Common;
using Nebukam.ORCA;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class EnemySystem : SystemBase
{
    public Transform player;
    public FSM p_fsm;
    private ORCABundle<Agent> m_simulation;
    public Dictionary<int, AIEntity> etLst = new Dictionary<int, AIEntity>(10000);
    float last_destroy;
    protected override void OnUpdate()
    {
        if (player == null)
        {
            var p = GameObject.Find("1001");
            if (p != null)
            {
                player = p.transform;
                p_fsm = player.GetComponent<FSM>();
                m_simulation = new ORCABundle<Agent>();
                m_simulation.plane = AxisPair.XZ;
            }
            else
            {
                return;
            }
        }

        if (wait_destroy.Count > 0 && UnityEngine.Time.time - last_destroy >= 2f)
        {
            last_destroy = UnityEngine.Time.time;
            for (int i = 0; i < wait_destroy.Count; i++)
            {
                var item = wait_destroy[i];
                var manage = World.EntityManager;
                var ac = manage.GetComponentData<AgentComponent>(item);
                if (UnityEngine.Time.time - ac.trigger_die >= 2)
                {
                    manage.DestroyEntity(item);
                    wait_destroy_remove.Add(item);
                }
            }

            if (wait_destroy_remove.Count > 0)
            {
                foreach (var item in wait_destroy_remove)
                {
                    wait_destroy.Remove(item);
                }
                wait_destroy_remove.Clear();
            }
        }


        EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        float3 p_pos = player.transform.position;
        var entityManager = World.EntityManager;
        var entites = entityManager.GetAllEntities(Unity.Collections.Allocator.Temp);
        foreach (var entity in entites)
        {
            if (entityManager.HasComponent<AgentComponent>(entity))
            {
                var ac = entityManager.GetComponentData<AgentComponent>(entity);
                if (ac.state == 0)//�մ��������ĵ�λ ��δ�������ó�ʼλ��
                {
                    //����λ�õĸ���
                    var lt = entityManager.GetComponentData<LocalTransform>(entity);
                    var f1 = UnityEngine.Random.Range(-25f, 25f);
                    var f2 = UnityEngine.Random.Range(-25f, 25f);
                    lt.Position = p_pos + new float3(f1, 0, f2);
                    entityManager.SetComponentData(entity, lt);
                    ac.state = 1;//��Ҫ�ƶ���������
                    entityManager.SetComponentData(entity, ac);

                    var a = m_simulation.NewAgent(lt.Position);
                    a.id = ac.id;
                    a.radius = 0.35f;//��λ�İ뾶
                    a.radiusObst = 0.35f;
                    a.maxSpeed = 1.75f;
                    a.prefVelocity = math.normalize(p_pos - a.pos) * 1.75f;
                    a.velocity = a.prefVelocity;
                    a.timeHorizon = 0.001f;

                    //�л����ƶ��Ķ���
                    var xx = new AIEntity(entity, ac, a, new NativeArray<byte>(1, Allocator.Persistent),
                           new NativeArray<quaternion>(1, Allocator.Persistent),
                           new NativeArray<float3>(1, Allocator.Persistent), lt,
                           entityManager.GetComponentData<GpuEcsAnimatorStateComponent>(entity), ac.unit_id);
                    etLst.Add(xx.GetInstanceID(), xx);
                    xx.Play(ref entityManager, AnimationIdsAnmState1001.run.GetHashCode());
                }
                else if (ac.state == 1)
                {
                    if (etLst.TryGetValue(ac.id, out var item))
                    {
                        float d = 1.5f;
                        //�ж������ǵľ��� 
                        if (item.atk_type == 1)
                        {
                            d = 15f;
                        }
                        if (math.distance(p_pos, item.ls.Position) <= d)
                        {
                            ac.state = 3;
                            item.send_bullet = false;//Ҫ�ڹ��������Ĺ�����,�ͷ��ӵ� 
                            entityManager.SetComponentData(entity, ac);

                            //������ʱ��������
                            item.ls.Rotation = Quaternion.LookRotation(p_pos - item.ls.Position);
                            entityManager.SetComponentData(entity, item.ls);

                            //���Ź�������
                            item.Play(ref entityManager, AnimationIdsAnmState1001.attack.GetHashCode());
                            item.a.maxSpeed = 0;//�ƶ��ٶ���0 ���⹥����ʱ�� ����λ��
                        }
                    }
                }
                else if (ac.state==2)//��λ�������߼�
                {
                    if (etLst.TryGetValue(ac.id, out var x))
                    {
                        m_simulation.agents.Remove(x.a);
                        x.Dispose();
                        etLst.Remove(ac.id);
                    }
                }
                else if (ac.state==3)
                {
                    //���ӵ���������
                     if (etLst.TryGetValue(ac.id, out var item))
                    {
                        var ee = entityManager.GetComponentData<GpuEcsAnimatorStateComponent>(entity);
                        //�ж��������ȵ��˶����� �����������֡,���������з�Χ��,����е�Ѫ����
                        if (ee.currentNormalizedTime >= 0.35f && ee.currentNormalizedTime <= 0.5f)
                        {
                            //0�ǽ�ս��λ
                            if (item.atk_type == 0)
                            {
                                if (item.send_bullet == false) {
                                    item.send_bullet = true;
                                    if (math.distance(p_pos, item.ls.Position) <= 1.5f)
                                    {
                                        //����ǽ�ս��λ,��ֱ���ܻ� ������ֱ�ӵ����ܻ�
                                        p_fsm.BeHit(item.ls.Position, item.ls.Forward());
                                    }
                                }
                            }
                            else if (item.atk_type == 1)
                            {
                                if (item.send_bullet == false)
                                {
                                    item.send_bullet = true;
                                    //��ȡ�����λ �չ�1������ Ȼ����ýӿڷ����ӵ�
                                    var n_atk_01_config = UnitStateManager.Instance.Get(item.unit_id, 1047);
                                    if (n_atk_01_config.effectConfigs != null && n_atk_01_config.effectConfigs.Count > 0)
                                    {
                                        Effect_Core.Instance.DO(n_atk_01_config.effectConfigs[0], ref item.ls, ref ecb);
                                    }
                                }
                            }
                        }
                        else if (ee.currentNormalizedTime >= 0.95f)
                        {
                            ac.state = 1;
                            item.a.maxSpeed = 1.75f;
                            entityManager.SetComponentData(entity, ac);
                            item.Play(ref entityManager, 1);
                        }
                    }

                }
            }
        }

        m_simulation.orca.TryComplete();
        m_simulation.orca.Schedule(SystemAPI.Time.DeltaTime);//����Ѱ·��ִ��job 

        if (etLst.Count > 0)
        {
            foreach (var item in etLst)
            {
                //ִ�и���λ�õ�job
                item.Value.DOUpdatePositionJob(p_pos, ecb);
            }
            foreach (var item in etLst)
            {
                item.Value.DOUpdatePosition(ecb);
            }
        }

        ecb.Playback(entityManager);
        ecb.Dispose();

    }

    internal AIEntity GetEntity(int id)
    {
        if (etLst.ContainsKey(id))
        {
            return etLst[id];
        }
        else
        {
            return null;
        }
    }

    public List<Entity> wait_destroy = new List<Entity>();
    public List<Entity> wait_destroy_remove = new List<Entity>();
    internal void Add_WaitDestroy(Entity e)
    {
        wait_destroy.Add(e);
    }
}

public class AIEntity
{
    public Entity e;//ͨ�����ʵ���ȡ���
    public AgentComponent ac;//��¼�˵�λ��һЩ��Ϣ
    public Agent a;//ȷ�� ��һ���ƶ������� (��̬����)

    //jobsystem �����ƶ�
    public NativeArray<byte> _rot_update;
    public NativeArray<quaternion> _rot;
    public NativeArray<float3> _prefVelocity;

    public LocalTransform ls;

    //GPU Animation
    public GpuEcsAnimatorControlComponent gpuEcsAnimatorControlComponent;
    public GpuEcsAnimatorControlStateComponent gpuEcsAnimatorControlStateComponent;
    public GpuEcsAnimatorStateComponent stateComponent;

    JobHandle _jobHandle;

    public int unit_id;//��λID
    public int atk_type;//0��ս 1Զ��

    public bool send_bullet;//�Ƿ��ѷ����ӵ�(����)

    public WorldUnit grildInfo;//��ʾ��ǰ���ڵ�ͼ�ĸ�����

    public int GetUnitId()
    {
        return ac.unit_id;
    }


    public AIEntity(Entity e, AgentComponent ac, Agent a, NativeArray<byte> rot_update,
        NativeArray<quaternion> rot, NativeArray<float3> prefVelocity, LocalTransform localTransform,
        GpuEcsAnimatorStateComponent stateComponent, int unit_id)
    {
        this.e = e;
        this.ac = ac;
        this.a = a;
        _rot_update = rot_update;
        _rot = rot;
        _prefVelocity = prefVelocity;
        this.ls = localTransform;
        this.unit_id = unit_id;
        atk_type = UnitData.Get(unit_id).atk_type;

        this.gpuEcsAnimatorControlComponent = new GpuEcsAnimatorControlComponent()
        {
            animatorInfo = new AnimatorInfo()
            {
                animationID = AnimationIdsAnmState1001.run.GetHashCode(),
                blendFactor = 0,
                speedFactor = 1
            },
            startNormalizedTime = 0,
            transitionSpeed = 0
        };

        this.gpuEcsAnimatorControlStateComponent = new GpuEcsAnimatorControlStateComponent()
        {
            state = GpuEcsAnimatorControlStates.Start,
            reset = true
        };

        this.stateComponent = stateComponent;
    }

    //��ȡ״̬
    public int GetState() {
        return ac.state;
    }

    //��ȡΨһID
    public int GetInstanceID()
    {
        return ac.id;
    }

    public bool IsActive()
    {
        return GetState() != -1;
    }

    //�ƶ� ����λ��  ..todo
    public void DOUpdatePositionJob(float3 p_pos, EntityCommandBuffer ecb)
    {
        if (GetState() == 1)
        {
            if (ac.unit_id == 1002)
            {
                EnemyJob enemyJob = new EnemyJob(p_pos, a.pos, ls.Position, 1.75f, _rot_update, _rot, _prefVelocity);
                _jobHandle = enemyJob.Schedule();
            }
            else if (ac.unit_id == 1003)
            {
                var r1 = p_pos - ls.Position;
                ls.Rotation = Unity.Mathematics.quaternion.LookRotation(r1, Vector3.up);
                ls.Position = ls.Position + ls.Forward() * Time.deltaTime * a.maxSpeed;
                ecb.SetComponent(e, ls);
            }

        }
    }

    public void DOUpdatePosition(EntityCommandBuffer ecb)
    {
        if (GetState() == 1)
        {
            if (ac.unit_id == 1002)
            {
                _jobHandle.Complete();
                if (_rot_update[0] == 1)
                {
                    ls.Rotation = _rot[0];
                }
                ls.Position = a.pos;
                ecb.SetComponent(e, ls);
                a.prefVelocity = _prefVelocity[0];
            }
            UpdateGrild();
        }
    }

    //��ͼ������.. ���ڿ���Ѱ��ָ������ĵ�λ
    public void UpdateGrild() {
        WorldUnitManager.Instance.Change(this);
    }

    //���Ŷ���
    public void Play(ref EntityManager entityManager, int id)
    {
        gpuEcsAnimatorControlComponent.animatorInfo.animationID = id;// AnimationIdsAnmState1001.attack.GetHashCode();
        gpuEcsAnimatorControlComponent.animatorInfo.blendFactor = 0;
        gpuEcsAnimatorControlComponent.animatorInfo.speedFactor = 1;

        gpuEcsAnimatorControlComponent.startNormalizedTime = 0;
        gpuEcsAnimatorControlComponent.transitionSpeed = 0;

        gpuEcsAnimatorControlStateComponent.state = GpuEcsAnimatorControlStates.Start;
        gpuEcsAnimatorControlStateComponent.reset = true;

        entityManager.SetComponentData(e, gpuEcsAnimatorControlComponent);
        entityManager.SetComponentData(e, gpuEcsAnimatorControlStateComponent);
    }


    internal Vector3 GetPosition()
    {
        return ls.Position;
    }

    public void Dispose()
    {
        _rot_update.Dispose();
        _rot.Dispose();
        _prefVelocity.Dispose();

        WorldUnitManager.Instance.Remove(this);
    }
}