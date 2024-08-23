using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Game.Config;

[CreateAssetMenu(menuName = "����/����״̬����")]
public class StateScriptableObject : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField]
    [ListDrawerSettings(ShowIndexLabels = true, ShowPaging = false, ListElementLabelName = "info")]
    public List<StateEntity> states = new List<StateEntity>();

    public void OnBeforeSerialize()
    {

    }

    public void OnAfterDeserialize()
    {
#if UNITY_EDITOR
        if (states.Count == 0)
        {

            Dictionary<int, PlayerStateEntity> dct = PlayerStateData.all;
            foreach (var item in dct)
            {
                var info = item.Value;
                StateEntity entity = new StateEntity();
                entity.id = info.id;
                entity.info = info.id + "_" + info.info;
                states.Add(entity);
            }
        }
        else
        {
            Dictionary<int, PlayerStateEntity> dct = PlayerStateData.all;
            if (dct.Count != states.Count)
            {
                //�����������״̬
                foreach (var item in dct)
                {
                    var info = item.Value;
                    bool add = true;
                    for (int i = 0; i < states.Count; i++)
                    {
                        if (states[i].id == info.id)
                        {
                            add = false;
                            continue;
                        }
                    }
                    //�������Ҫ����
                    if (add == true)
                    {
                        StateEntity stateEntity = new StateEntity();
                        stateEntity.id = info.id;
                        stateEntity.info = info.id + "_" + info.info;
                        states.Add(stateEntity);
                    }
                }
                List<StateEntity> remove = new List<StateEntity>();
                //ɾ���������
                foreach (var item in states)
                {
                    if (dct.ContainsKey(item.id) == false)
                    {
                        remove.Add(item);
                        //UDebug.LogError(remove.Count);
                    }
                }

                foreach (var item in remove)
                {
                    states.Remove(item);
                }
            }

        }
#endif
    }
}

[System.Serializable]
public class StateEntity
{
    public int id;
    public string info;

    [Header("����λ������")]
    public List<PhysicsConfig> physicsConfig;

    [Header("��Ч����")]
    [ListDrawerSettings(ShowIndexLabels = true, ShowPaging = false, ListElementLabelName = "trigger")]
    public List<EffectConfig> effectConfigs;
}

[System.Serializable]
public class PhysicsConfig
{
    [Header("������")]
    public float trigger;
    [Header("������")]
    public float time;//������
    [Header("λ�ƾ���")]
    public Vector3 force;
    [Header("��������")]
    public AnimationCurve cure = AnimationCurve.Constant(0, 1, 1);
    [Header("�Ƿ��������")]
    public bool ignore_gravity;


    [Header("��⵽��λ��ͣ��")]
    public float stop_dst;
}

[System.Serializable]
public class EffectConfig
{
    [Header("��Դ·��")]
    public string res_path;

    [Header("�ӵ�ID")]
    public int id;

    [Header("�����ټ�����")]
    public int level;

    [Header("������")]
    public float trigger;
    [Header("��ǰʹ�õĵ���ID")]
    public int use_prop_id;//0��û�и��������ж�
    [Header("���ɷ�ʽ:0���� 1�෢ɢ�� 2���ζ��� 3��Χ�����")]
    public int create_type;
    [Header("���з�Χ")]
    public float hit_range = 1;

    [Space(15)]
    [Header("����-�ο�λ��:0���� 1Ŀ��")]
    public int spawn_point_type;
    [Header("����-�ҵ�����")]
    public string spawn_hang_point;
    [Header("����-����:0�ҵ㷽�� 1����Ŀ�� 2����ǰ�� 3Ŀ��ǰ��")]
    public int rotate_type;
    [Header("����-λ��ƫ��")]
    public Vector3 position_offset;


    [Space(15)]
    [Header("����-��������(���Ҷ���)")]
    public int fan_count;
    [Header("����-����Ƕ�")]
    public int fan_angle_difference;

    [Space(15)]
    [Header("����-���ɶ�����")]
    public int rect_rows;
    [Header("����-���ɶ�����")]
    public int rect_columns;
    [Header("����-ÿ�м��")]
    public float rect_rows_spacing;
    [Header("����-ÿ�м��")]
    public float rect_columns_spacing;

    [Space(15)]
    [Header("�����Χ-�뾶(��С)")]
    public float random_radius;
    [Header("�����Χ-�뾶(���)")]
    public float random_radius_max;
    [Header("�����Χ-����")]
    public int random_count;
    [Header("�����Χ-����(���)")]
    public int random_count_max;

    [Header("�����Χ-�Ƕ�")]
    public float random_angle;
    [Header("�����Χ-�Ƕ�(���)")]
    public float random_angle_max;

    [Space(30)]
    [Header("�ƶ��ķ�ʽ:0�������ƶ� 1׷���ƶ� 2Χ����ת 3�����������ƶ�")]
    public byte move_type;
    [Header("������-�ƶ�")]
    public DirectMoveConfg directMoveConfg;

    [Header("׷��Ŀ��-�ƶ�")]
    public TrackMoveConfig trackMoveConfig;
    [Header("Χ����ת-�ƶ�")]
    public AroundMoveConfig aroundMoveConfig;
    [Header("����-�ƶ�")]
    public BezierCurveMoveConfig bezierCurveMoveConfig;

    [Space(30)]
    [Header("����ʱ����Ч")]
    public string hit_effect;
    [Header("����ʱ��Ч����:0������ 1ֻ����һ��")]
    public int hit_effect_count;
    [Header("����ʱ����Ч")]
    public string hit_audio;

    [Space(30)]
    [Header("����-���ʱ��")]
    public float destroy_durtaion;

    [Header("����-���ж��ٸ���λ")]
    public int destroy_hit_count;

    [Header("����-��������Ա�")]
    public float destroy_self_explosion;



}

[System.Serializable]
public class DirectMoveConfg
{

    [Header("�ƶ��ٶ�")]
    public float speed;
    [Header("���ٶ�/ÿ��")]
    public float acceleration;
    [Header("����ٶ�")]
    public float maxSpeed;

    [Header("�Ƿ��Զ����ƶ�����")]
    public bool custom_direction;
    [Header("�Զ���ķ���")]
    public Vector3 direction;
    [Header("��������,0�������� 1��������")]
    public int space;

}

[System.Serializable]
public class TrackMoveConfig
{
    [Header("�ƶ��ٶ�")]
    public float speed;

    [Header("Ť��")]
    public float torque;

    [Header("���־���")]
    public float stopDistance;

    [Header("����X��")]
    public bool x_freeze;

    [Header("����Z��")]
    public bool z_freeze;
}

[System.Serializable]
public class AroundMoveConfig
{
    [Header("�ƶ��ٶ�")]
    public float speed;

    [Header("����Ŀ����ٶ�")]
    public float follow_speed;
}

[System.Serializable]
public class BezierCurveMoveConfig
{

    [Header("ʱ��")]
    public float duration;
    [Header("����:0���� 1����")]
    public int type;
    [Header("��һ�����Ƶ����λ��")]
    public Vector3 p1;

    [Header("�ڶ������Ƶ����λ��")]
    public Vector3 p2;

    [Header("�յ�-���λ��")]
    public Vector3 end;
}