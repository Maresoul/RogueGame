using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class FSMServiceBase
{
    public FSM player;

    public virtual void Init(FSM fsm)
    {
        player = fsm;
    }

    //状态开始的时候 会调度这个接口
    public virtual void OnBegin(PlayerState state)
    {

    }

    //每一帧更新
    public virtual void OnUpdate(float normalizedTime, PlayerState state) { }

    //退出状态的时候
    public virtual void OnEnd(PlayerState state) { }

    //状态对应的动画结束的时候,会进行调度
    public virtual void OnAnimationEnd(PlayerState state) { }

    //当重新播放动画的时候
    public virtual void ReStart(PlayerState state) { }

    //当它重新循环 (技能等级-播放多次 射箭技能 可能会重复五次--)
    public virtual void ReLoop(PlayerState state) { }


    public virtual void OnDisable(PlayerState state) { }

    //key 是index bool 执行过
    public Dictionary<int, bool> executed = new Dictionary<int, bool>();

    public void SetExcuted(int index)
    {
        executed[index] = true;
    }

    public void ReSetExcuted(int index)
    {
        executed[index] = false;
    }

    public void ReSetAllExcuted()
    {
        if (executed.Count > 0)
        {
            executed.Clear();
        }
    }

    public bool GetExcuted(int index)
    {
        if (executed.TryGetValue(index, out var v))
        {
            return v;
        }
        return false;
    }
}
