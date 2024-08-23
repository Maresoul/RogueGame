using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class AnimationService : FSMServiceBase
{
    public float normalizedTime;//��ǰ�������ŵĽ���

    public string now_play_id;//��һ�㲥�ŵĶ���ID
    public string now_play_id2;//�ڶ��㲥�ŵ�ID
    public override void Init(FSM fsm)
    {
        base.Init(fsm);
    }

    public override void OnAnimationEnd(PlayerState state)
    {
        base.OnAnimationEnd(state);
    }

    void Play(PlayerState state)
    {
        //normalizedTime = 0;
        //now_play_id = state.excel_config.anm_name;
        //player.animator.Play(state.excel_config.anm_name,0, 0);
        //player.animator.Update(0);

        if (string.IsNullOrEmpty(state.excel_config.anm_name2))
        {
            normalizedTime = 0;
            now_play_id = state.excel_config.anm_name;
            player.animator.Play(state.excel_config.anm_name, 0, 0);

            now_play_id2 = "None";
            player.animator.Play("None", 1, 0);
        }
        else
        {
            normalizedTime = 0;
            if (now_play_id != state.excel_config.anm_name)
            {
                now_play_id = state.excel_config.anm_name;
                player.animator.Play(state.excel_config.anm_name, 0, 0);
            }

            now_play_id2 = state.excel_config.anm_name2;
            //Debug.LogError(state.excel_config.anm_name2);
            player.animator.Play(state.excel_config.anm_name2, 1, 0);
        }
        player.animator.Update(0);
    }

    public override void OnBegin(PlayerState state)
    {
        base.OnBegin(state);
        call_end = false;
        Play(state);
    }

    public override void OnDisable(PlayerState state)
    {
        base.OnDisable(state);
    }

    public override void OnEnd(PlayerState state)
    {
        base.OnEnd(state);
    }

    bool call_end;
    public override void OnUpdate(float normalizedTime, PlayerState state)
    {
        base.OnUpdate(normalizedTime, state);

        //�ж����ŵĶ��� �Ƿ���������õ�״̬�Ķ��� ��һ�µ� 
        if (!string.IsNullOrEmpty(now_play_id))
        {
            if (string.IsNullOrEmpty(state.excel_config.anm_name2))//����ڶ���û������
            {
                AnimatorStateInfo info = player.animator.GetCurrentAnimatorStateInfo(0);

                if (info.IsName(now_play_id))
                {
                    //0-1 ��ʾ����0%-100%�Ľ���
                    this.normalizedTime = info.normalizedTime;
                    if (normalizedTime > 1)
                    {
                        //0.99  1.02
                        if (call_end == false)
                        {
                            this.normalizedTime = 1;
                            player.AnimationOnPlayEnd();
                            call_end = true;
                        }
                        else
                        {
                            if (call_end == true) { call_end = false; }
                            this.normalizedTime = normalizedTime % 1;
                        }
                    }
                }
                else
                {
                    this.normalizedTime = 0;
                }

            }
            else
            {
                AnimatorStateInfo info = player.animator.GetCurrentAnimatorStateInfo(1);

                if (info.IsName(now_play_id2))
                {
                    //0-1 ��ʾ����0%-100%�Ľ���
                    this.normalizedTime = info.normalizedTime;
                    if (normalizedTime > 1)
                    {
                        //0.99  1.02
                        if (call_end == false)
                        {
                            this.normalizedTime = 1;
                            player.AnimationOnPlayEnd();
                            call_end = true;
                        }
                        else
                        {
                            if (call_end == true) { call_end = false; }
                            this.normalizedTime = normalizedTime % 1;
                        }
                    }
                }
                else
                {
                    this.normalizedTime = 0;
                }
            }
        }
    }

    public override void ReLoop(PlayerState state)
    {
        base.ReLoop(state);
    }

    public override void ReStart(PlayerState state)
    {
        base.ReStart(state);
    }
}
