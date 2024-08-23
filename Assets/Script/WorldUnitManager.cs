using System;
using System.Collections;
using System.Collections.Generic;

using Unity.Entities;
using Unity.Mathematics;

using UnityEngine;

public class WorldUnitManager
{
    static WorldUnitManager instance = new WorldUnitManager();
    public static WorldUnitManager Instance => instance;

    public int width = 1;
    int customId = 0;

    public Dictionary<int, Dictionary<int, WorldUnit>> grilds_dct = new Dictionary<int, Dictionary<int, WorldUnit>>();


    public void Clear()
    {
        grilds_dct.Clear();
        foreach (var item in grilds_dct)
        {
            if (item.Value.Count == 0)
            {
                continue;
            }
            foreach (var item2 in item.Value)
            {
                item2.Value.Clear();
            }
        }
        grilds_dct.Clear();
        customId = 0;
    }


    //判断这个坐标是否有对应的格子了
    public WorldUnit HasGrild(Vector3 p)
    {
        var x = (int)Math.Floor(p.x / width);//3.14 3  -2.8 -3
        var z = (int)Math.Floor(p.z / width);
        if (grilds_dct.TryGetValue(z, out var z_grilds))
        {
            if (z_grilds.TryGetValue(x, out var x_grild))
            {
                return x_grild;
            }
        }
        return null;
    }

    //根据坐标获取格子
    public WorldUnit Get(Vector3 position)
    {
        var p = position;
        //向下取整
        var x = (int)Math.Floor(p.x / width);
        var z = (int)Math.Floor(p.z / width);
        if (grilds_dct.ContainsKey(z) == false)
        {
            grilds_dct[z] = new Dictionary<int, WorldUnit>();
        }

        if (grilds_dct[z].TryGetValue(x, out var g))
        {
            return g;
        }
        else
        {
            var x_min = x * width;
            var z_min = z * width;
            customId += 1;
            WorldUnit item = new WorldUnit(customId, (float)x_min, (float)(x_min + width),
                (float)z_min, (float)(z_min + width), width);

            item.z_id = z;//行ID 前后
            item.x_id = x;//列ID 左右
            grilds_dct[z][x] = item;
            return item;
        }
    }


    //设置AI对应的格子
    public WorldUnit Set(AIEntity p)
    {
        if (p != null)
        {
            var g = Get(p.GetPosition());
            if (g != null)
            {
                g.Add(p);
                p.grildInfo = g;
                return g;
            }
        }
        return null;
    }

    //单位移动后,会调用这个接口,更新格子ID
    public WorldUnit Change(AIEntity p)
    {
        if (p != null)
        {
            if (p.grildInfo != null)
            {
                var result = p.grildInfo.ContainPoint(p.GetPosition());
                if (result == false)
                {
                    p.grildInfo.Remove(p);
                    return Set(p);
                }
                else
                {
                    return p.grildInfo;
                }
            }
            else
            {
                return Set(p);
            }
        }
        return null;
    }

    //移除
    public void Remove(AIEntity p)
    {
        if (p != null && p.grildInfo != null)
        {
            var result = p.grildInfo.Remove(p);
            if (result)
            {
                p.grildInfo = null;
            }
        }
    }

    //根据范围获取格子的数量
    public int GetCount(float range)
    {
        int count = (int)Math.Ceiling(range / width);//向上取整 范围是10.5/1 ==11个格子
        return count;
    }

    //根据范围,获取一定范围内的给子的对象
    public void GetRangTarget(WorldUnit g, float range, Action<AIEntity> a)
    {
        //感兴趣的格子数量
        int count = GetCount(range);
        //取到左下角的位置
        int z = g.z_id - count;
        int x = g.x_id - count;
        //数量乘以2
        var count1 = count * 2;
        if (grilds_dct.Count>0)
        {
            for (int j = 0; j <= count1; j++)
            {
                var z1 = z + j;
                //如果这一行不存在  直接执行下一行的判断
                if (grilds_dct.ContainsKey(z1) == false) { continue; }

                for (int k = 0; k <= count1; k++)
                {
                    var x1 = x + k;
                    if (grilds_dct[z1].ContainsKey(x1) == false) { continue; }

                    var item = grilds_dct[z1][x1];


                    if (item.unitDct.Count > 0)
                    {
                        foreach (var pp in item.unitDct.Values)
                        {
                            a?.Invoke(pp);
                        }
                    }
                }
            }

        }
    }

    internal void OnHit(float3 bullet_pos, float range, EntityManager manage, EnemySystem es, Action<EntityManager, EnemySystem, AIEntity, float3, float, int> a, int bulletId, bool on_hit_destroy)
    {
        //var t = grilds_dct.Values;
        int count = GetCount(range);

        var x_id = (int)Math.Floor(bullet_pos.x / width);
        var z_id = (int)Math.Floor(bullet_pos.z / width);

        int z = z_id - count;
        int x = x_id - count;
        var count1 = count * 2;
        if (grilds_dct.Count > 0)
        {
            for (int j = 0; j <= count1; j++)
            {
                var z1 = z + j;
                if (grilds_dct.ContainsKey(z1) == false) { continue; }

                for (int k = 0; k <= count1; k++)
                {
                    var x1 = x + k;
                    if (grilds_dct[z1].ContainsKey(x1) == false) { continue; }

                    var item = grilds_dct[z1][x1];

                    if (item.unitDct.Count > 0)
                    {
                        foreach (var pp in item.unitDct.Values)
                        {
                            a?.Invoke(manage, es, pp, bullet_pos, range, bulletId);
                            if (on_hit_destroy)
                            {
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}


public class WorldUnit
{
    public int id;//格子的ID

    //public int y_id;
    public int x_id;
    public int z_id;

    public float x_min;//格子最左边x
    //public float y_min;
    public float z_min;//格子的最小z

    public float x_max;
    //public float y_max;
    public float z_max;

    public float size;//每个格子的大小

    //public Vector3 pos;

    //public Dictionary<string, PlayerData> unit = new Dictionary<string, PlayerData>();

    public Dictionary<int, AIEntity> unitDct = new Dictionary<int, AIEntity>(50);

    public WorldUnit(int id, float x_min, float x_max, float z_min, float z_max, int size)
    {
        this.id = id;
        this.x_min = x_min;
        this.z_min = z_min;

        //pos = new Vector3(x_min, z_min);
        this.x_max = x_max;
        this.z_max = z_max;
        this.size = size;
    }
    //清除格子上缓存的所有单位
    public void Clear()
    {
        if (unitDct.Count > 0)
        {
            unitDct.Clear();
        }
    }

    //往这个格子添加单位
    public bool Add(AIEntity pd)
    {
        if (pd == null)
        {
            return false;
        }
        unitDct[pd.GetInstanceID()] = pd;
        //pd.grildInfo = this;
        return true;
    }

    public bool Remove(int _instance_id)
    {
        var id = _instance_id;
        if (unitDct.ContainsKey(id))
        {
            unitDct.Remove(id);
            return true;
        }
        return false;
    }

    public bool Remove(AIEntity pd)
    {
        if (pd == null)
        {
            return false;
        }
        var id = pd.GetInstanceID();
        if (unitDct.ContainsKey(id))
        {
            unitDct.Remove(id);
            //pd.grildInfo = null;
            return true;
        }
        return false;
    }

    //判断这个位置是否在这个格子内部
    public bool ContainPoint(Vector3 point)
    {
        if (point.x >= x_min && point.x <= x_max && point.z >= z_min && point.z <= z_max)
        {
            return true;
        }
        return false;
    }

    //获取周围八个格子
    //public void AddToList(List<AIEntity> lst)
    //{
    //    if (lst != null && unitDct.Count > 0)
    //    {
    //        var valus = unitDct.Values;
    //        foreach (var item in valus)
    //        {
    //            if (item != null && item.IsActive())
    //            {
    //                lst.Add(item);
    //            }
    //        }
    //    }
    //}

}