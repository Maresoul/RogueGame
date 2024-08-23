using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Game.Config;

[CreateAssetMenu(menuName = "配置/创建状态配置")]
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
                //遍历表格所有状态
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
                    //如果是需要增加
                    if (add == true)
                    {
                        StateEntity stateEntity = new StateEntity();
                        stateEntity.id = info.id;
                        stateEntity.info = info.id + "_" + info.info;
                        states.Add(stateEntity);
                    }
                }
                List<StateEntity> remove = new List<StateEntity>();
                //删除掉多余的
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

    [Header("物理位移配置")]
    public List<PhysicsConfig> physicsConfig;

    [Header("特效配置")]
    [ListDrawerSettings(ShowIndexLabels = true, ShowPaging = false, ListElementLabelName = "trigger")]
    public List<EffectConfig> effectConfigs;
}

[System.Serializable]
public class PhysicsConfig
{
    [Header("触发点")]
    public float trigger;
    [Header("结束点")]
    public float time;//结束点
    [Header("位移距离")]
    public Vector3 force;
    [Header("曲线配置")]
    public AnimationCurve cure = AnimationCurve.Constant(0, 1, 1);
    [Header("是否忽略重力")]
    public bool ignore_gravity;


    [Header("检测到单位后停下")]
    public float stop_dst;
}

[System.Serializable]
public class EffectConfig
{
    [Header("资源路径")]
    public string res_path;

    [Header("子弹ID")]
    public int id;

    [Header("当多少级调用")]
    public int level;

    [Header("触发点")]
    public float trigger;
    [Header("当前使用的道具ID")]
    public int use_prop_id;//0则没有该条件的判断
    [Header("生成方式:0单发 1多发散射 2矩形队列 3范围内随机")]
    public int create_type;
    [Header("命中范围")]
    public float hit_range = 1;

    [Space(15)]
    [Header("出生-参考位置:0自身 1目标")]
    public int spawn_point_type;
    [Header("出生-挂点名称")]
    public string spawn_hang_point;
    [Header("出生-朝向:0挂点方向 1朝向目标 2自身前方 3目标前方")]
    public int rotate_type;
    [Header("出生-位置偏差")]
    public Vector3 position_offset;


    [Space(15)]
    [Header("扇形-生成数量(左右多少)")]
    public int fan_count;
    [Header("扇形-间隔角度")]
    public int fan_angle_difference;

    [Space(15)]
    [Header("矩形-生成多少行")]
    public int rect_rows;
    [Header("矩形-生成多少列")]
    public int rect_columns;
    [Header("矩形-每行间隔")]
    public float rect_rows_spacing;
    [Header("矩形-每列间隔")]
    public float rect_columns_spacing;

    [Space(15)]
    [Header("随机范围-半径(最小)")]
    public float random_radius;
    [Header("随机范围-半径(最大)")]
    public float random_radius_max;
    [Header("随机范围-数量")]
    public int random_count;
    [Header("随机范围-数量(最大)")]
    public int random_count_max;

    [Header("随机范围-角度")]
    public float random_angle;
    [Header("随机范围-角度(最大)")]
    public float random_angle_max;

    [Space(30)]
    [Header("移动的方式:0按方向移动 1追踪移动 2围绕旋转 3贝塞尔曲线移动")]
    public byte move_type;
    [Header("按方向-移动")]
    public DirectMoveConfg directMoveConfg;

    [Header("追踪目标-移动")]
    public TrackMoveConfig trackMoveConfig;
    [Header("围绕旋转-移动")]
    public AroundMoveConfig aroundMoveConfig;
    [Header("曲线-移动")]
    public BezierCurveMoveConfig bezierCurveMoveConfig;

    [Space(30)]
    [Header("命中时的特效")]
    public string hit_effect;
    [Header("命中时特效数量:0无限制 1只创建一次")]
    public int hit_effect_count;
    [Header("命中时的音效")]
    public string hit_audio;

    [Space(30)]
    [Header("销毁-存活时间")]
    public float destroy_durtaion;

    [Header("销毁-命中多少个单位")]
    public int destroy_hit_count;

    [Header("销毁-多少秒后自爆")]
    public float destroy_self_explosion;



}

[System.Serializable]
public class DirectMoveConfg
{

    [Header("移动速度")]
    public float speed;
    [Header("加速度/每秒")]
    public float acceleration;
    [Header("最大速度")]
    public float maxSpeed;

    [Header("是否自定义移动方向")]
    public bool custom_direction;
    [Header("自定义的方向")]
    public Vector3 direction;
    [Header("坐标类型,0世界坐标 1本地坐标")]
    public int space;

}

[System.Serializable]
public class TrackMoveConfig
{
    [Header("移动速度")]
    public float speed;

    [Header("扭矩")]
    public float torque;

    [Header("保持距离")]
    public float stopDistance;

    [Header("冻结X轴")]
    public bool x_freeze;

    [Header("冻结Z轴")]
    public bool z_freeze;
}

[System.Serializable]
public class AroundMoveConfig
{
    [Header("移动速度")]
    public float speed;

    [Header("跟随目标的速度")]
    public float follow_speed;
}

[System.Serializable]
public class BezierCurveMoveConfig
{

    [Header("时长")]
    public float duration;
    [Header("类型:0二阶 1三阶")]
    public int type;
    [Header("第一个控制点相对位置")]
    public Vector3 p1;

    [Header("第二个控制点相对位置")]
    public Vector3 p2;

    [Header("终点-相对位置")]
    public Vector3 end;
}