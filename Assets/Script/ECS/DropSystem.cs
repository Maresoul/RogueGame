using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class DropSystem : SystemBase
{
    List<Entity> e = new List<Entity>(100);
    List<Entity> wait_destroy = new List<Entity>(100);
    public FSM p_fsm;
    public void Add(Entity entity)
    {
        e.Add(entity);
    }


    protected override void OnUpdate()
    {
        if (p_fsm == null)
        {
            var p = GameObject.Find("1001");
            if (p != null)
            {
                p_fsm = p.GetComponent<FSM>();
            }
            else
            {
                return;
            }
        }

        if (e.Count > 0)
        {
            for (int i = 0; i < e.Count; i++)
            {
                var item = e[i];
                var LT = EntityManager.GetComponentData<LocalTransform>(item);
                //<1.5 添加到待销毁的容器
                if (math.abs(LT.Position.x - p_fsm.transform.position.x) <= 1.5f && math.abs(LT.Position.z - p_fsm.transform.position.z) <= 1.5f)
                {
                    wait_destroy.Add(item);
                }
            }
            //遍历待销毁的容器
            if (wait_destroy.Count > 0)
            {
                for (int i = 0; i < wait_destroy.Count; i++)
                {
                    var item = wait_destroy[i];
                    e.Remove(item);
                    EntityManager.DestroyEntity(item);
                    p_fsm.GetDrop();
                }
                wait_destroy.Clear();
            }
        }
    }
}
