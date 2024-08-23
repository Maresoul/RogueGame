using System;
using System.Collections.Generic;
using Game.Config;
using RootMotion.FinalIK;

using Unity.Mathematics;

using UnityEngine;

public class FSM : MonoBehaviour
{
    public bool AI;
    public int id;//单位的ID
    public UnitEntity unitEntity;//单位基础表

    public PlayerState currentState;
    Dictionary<int, PlayerState> stateData = new Dictionary<int, PlayerState>();

    public Animator animator;
    public UnityEngine.CharacterController characterController;


    public UnitAttEntity att_base;//总属性
    public UnitAttEntity att_crn;//当前属性==>生命值


    [HideInInspector]
    public Transform _transform;
    [HideInInspector]
    public GameObject _gameObject;
    StateScriptableObject anmConfig;

    public AimController aimController_right;
    public AimIK aimIK_right;
    Transform target_ik;
    private void Awake()
    {
        CCControllerEvent.NtkButtonOnClick = NtkButtonOnClick;
        _transform = this.transform;
        _gameObject = this.gameObject;

        aimIK_right = this.GetComponent<AimIK>();
        aimController_right = this.GetComponent<AimController>();
        var t_ik = new GameObject("ik_target");
        target_ik = t_ik.transform;

        animator = _transform.GetChild(0).GetComponent<Animator>();
        characterController = GetComponent<UnityEngine.CharacterController>();

        unitEntity = UnitData.Get(id);
        ServiceInit();
        StateInit();

        ToNext(1001);
    }

    private void NtkButtonOnClick(int type)
    {
        if (type==0)
        {
            OnAtk();
        }
        else if (type == 1)
        {
            ToNext(1046);
        }
        else if (type == 2)
        {
            ToNext(1047);
        }
        else if (type == 3)
        {
            ToNext(1048);
        }
        else if (type == 4)
        {
            ToNext(1049);
        }
    }

    void StateInit() {
        anmConfig = Resources.Load<StateScriptableObject>($"StateConfig/{id}");


        Dictionary<int, StateEntity> state_config = new Dictionary<int, StateEntity>();
        foreach (var item in anmConfig.states)
        {
            state_config[item.id] = item;
        }

        //所有的动作剪辑 
        var clips = animator.runtimeAnimatorController.animationClips;
        Dictionary<string, float> clipLength = new Dictionary<string, float>();
        foreach (var clip in clips)
        {
            clipLength[clip.name] = clip.length;
        }

        //遍历EXCEL
        if (PlayerStateData.all != null)
        {
            foreach (var item in PlayerStateData.all)
            {
                PlayerState p = new PlayerState();
                p.excel_config = item.Value;
                p.id = item.Key;
                p.stateEntity = state_config[p.id];
                if (clipLength.TryGetValue(item.Value.anm_name, out var length_clip))
                {
                    p.clipLength = length_clip;
                }
                stateData[item.Key] = p;
            }
        }

        //绑定的事件: (空格)位移   (鼠标左键)普攻 (wasd)移动 (1234)放四种道具:炸弹 飞镖 飞刀 飞轮  wasd抬起的时候,停止移动
        //
        if (AI==false)
        {
            foreach (var item in stateData)
            {
                if (item.Value.excel_config.on_move != null)
                {
                    AddListener(item.Key, StateEventType.update, OnMove);
                }

                if (item.Value.excel_config.do_move == 1)
                {
                    AddListener(item.Key, StateEventType.update, PlayerMove);
                }

                if (item.Value.excel_config.on_stop != 0)
                {
                    AddListener(item.Key, StateEventType.update, OnStop);
                }

                if (item.Value.excel_config.on_sprint != null)
                {
                    AddListener(item.Key, StateEventType.update, OnSprint);
                }

                //1 2 3 4
                if (item.Value.excel_config.on_use_prop != null)
                {
                    AddListener(item.Key, StateEventType.update, UseProp);
                }

                if (item.Value.excel_config.on_atk != null)
                {
                    AddListener(item.Key, StateEventType.update, OnAtk);
                }
            }
            AddListener(1051, StateEventType.update, OpenRightAimIK);
            AddListener(1051, StateEventType.end, CloseRightAimIK);
        }
    }

    private void OpenRightAimIK()
    {
        if (animationService.normalizedTime >= 0f)
        {
            if (aimController_right.target == null)
            {
                aimIK_right.solver.target = target_ik;
                aimController_right.target = target_ik;
            }
            aimController_right.weight = 1;
            target_ik.transform.position = this.transform.position + transform.forward * 5f + Vector3.up;
        }
    }

    private void CloseRightAimIK()
    {
        aimIK_right.solver.IKPositionWeight = 0;

        aimController_right.weight = 0;
    }


    private void OnAtk()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (CheckConfig(currentState.excel_config.on_normal_atk))
            {
                ToNext((int)currentState.excel_config.on_normal_atk[2]);
            }
        }
    }

        public void UseProp()
    {
        if (CheckConfig(currentState.excel_config.on_use_prop))
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ToNext(1046);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ToNext(1047);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                ToNext(1048);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                ToNext(1049);
            }
        }
    }

    private void OnSprint()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (CheckConfig(currentState.excel_config.on_sprint))
            {
                ToNext((int)currentState.excel_config.on_sprint[2]);
            }
        }
    }


    private void OnStop()
    {
        if (GetMoveInput()==false)// (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
        {
            //jump_count = 0;
            ToNext(currentState.excel_config.on_stop);
        }
    }

    float _targetRotation;
    float _rotationVelocity;
    float RotationSmoothTime = 0.05f;
    public float _speed = 5;
    private void PlayerMove()
    {
        //if (GetMoveInput()==false) {
        //    return;
        //}
        var v= GetMoveInputValue();
        var x = v.x;// Input.GetAxis("Horizontal");
        var z = v.y;// Input.GetAxis("Vertical");
        if (x != 0 || z != 0)
        {
            Vector3 inputDirection = new Vector3(x, 0f, z).normalized;

            //Mathf.Atan2 正切函数 求弧度 * Mathf.Rad2Deg(弧度转度数) >> 度数

            //领
            //第一:先求出输入的角度
            //第二:加上当前相机的Y轴旋转的量
            //第三:得到目标朝向的角度
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;// +GameDefine._Camera.eulerAngles.y;

            //做一个插值运动
            //float rotation = Mathf.SmoothDampAngle(_transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
            //    RotationSmoothTime);

            float y = Mathf.LerpAngle(transform.eulerAngles.y, /*rotation*/_targetRotation, Time.deltaTime * 180);
            this.transform.eulerAngles = new Vector3(0, y, 0);

            //角色先旋转到目标角度去
            // rotate to face input direction relative to camera position
            //_transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

            //计算目标方向 通过这个角度
            //Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
            //Move(targetDirection.normalized * (_speed * GameTime.deltaTime), false, false, false, true);
            Move(inputDirection * GetMoveSpeed(), false);
        }
    }

    public float GetMoveSpeed()
    {
        return _speed;
    }


    /// <summary>
    /// 状态切换关系的检查
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    public bool CheckConfig(float[] config)
    {
        if (config == null)
        {
            return false;
        }
        else
        {
            //还处于前摇部分 或者已经进入后摇阶段了 那表示可以进行切换
            if ((animationService.normalizedTime >= 0 && animationService.normalizedTime <= config[0])
                        || animationService.normalizedTime >= config[1])
            {
                return true;
            }
            return false;
        }
    }

    public bool GetMoveInput() {

        if (CCControllerEvent.GetJoystick!=null&& CCControllerEvent.GetJoystick.Invoke()!=Vector2.zero)
        {
            return true;
        }
        else if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            return true;
        }
        return false;
    }


    public Vector2 GetMoveInputValue()
    {
        if (CCControllerEvent.GetJoystick != null && CCControllerEvent.GetJoystick.Invoke() != Vector2.zero)
        {
            return CCControllerEvent.GetJoystick.Invoke();
        }
        else if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }
        return Vector2.zero;
    }


    private void OnMove()
    {
        //监听硬件的输入
        if (GetMoveInput())// (Input.GetAxis("Horizontal")!=0|| Input.GetAxis("Vertical") != 0)
        {
            if (CheckConfig(currentState.excel_config.on_move))
            {
                ToNext((int)currentState.excel_config.on_move[2]);
            }
        }
    }

    void Start()
    {
        
    }


    void Update()
    {
        if (currentState != null)
        {
            if (ServicesOnUpdate() == true)
            {
                DOStateEvent(currentState.id, StateEventType.update);//状态每帧执行的事件
            }

            //ToGround();
        }
    }

    #region 状态切换

    #endregion
    public bool ToNext(int next)
    {
        if (stateData.ContainsKey(next))
        {
            //if (currentState != null)
            //{
            //    Debug.Log($"{this.gameObject.name}:切换状态:{stateData[next].Info()}  当前是:{currentState.Info()}");
            //}
            //else
            //{
            //    Debug.Log($"{this.gameObject.name}:切换状态:{stateData[next].Info()}");
            //}

            //下一个状态 是否还处于CD中
            var next_state = stateData[next];
            //if (next_state.skill != null && next_state.begin != 0 && GameTime.time - next_state.begin < next_state.skill.cd)
            //{
            //    MainViewController.Instance.OpenCD_Tips();
            //    return false;
            //}

            if (currentState != null)
            {
                DOStateEvent(currentState.id, StateEventType.end);//状态绑定的退出事件
                ServicesOnEnd();

                //Debug.LogError(currentState.id+"   "+ next_state.id);
            }

            //pow_atk_begin = 0;

            //if (next_state != null && AI == false)
            //{
            //    SkillFx(next_state);
            //}

            currentState = next_state;

            //if (!(currentState.id == 1020 || currentState.id == 1021))
            //{
            //    jump_count = 0;
            //}
            currentState.SetBeginTime();

            ServicesOnBegin();
            DOStateEvent(currentState.id, StateEventType.begin); //执行当前状态的开始(进入)事件
            return true;
        }
        return false;
    }
    public List<FSMServiceBase> fSMServices = new List<FSMServiceBase>();

    AnimationService animationService;
    PhysicsService physicsService;
    EffectService effectService;
    int service_count;
    public T AddService<T>() where T : FSMServiceBase, new()
    {
        T com = new T();
        fSMServices.Add(com);
        com.Init(this);
        return com;
    }

    //注册 添加不同服务组件
    public void ServiceInit()
    {
        animationService = AddService<AnimationService>();
        physicsService = AddService<PhysicsService>();
        effectService=AddService<EffectService>();
        service_count = fSMServices.Count;
    }



    public void ServicesOnBegin()
    {
        for (int i = 0; i < service_count; i++)
        {
            fSMServices[i].OnBegin(currentState);
        }
    }

    public bool ServicesOnUpdate()
    {
        int crn_state_id = currentState.id;
        for (int i = 0; i < service_count; i++)
        {
            fSMServices[i].OnUpdate(animationService.normalizedTime, currentState);
            if (currentState.id != crn_state_id)
            {
                return false;
            }
        }
        return true;
    }

    public void ServicesOnEnd()
    {
        for (int i = 0; i < service_count; i++)
        {
            fSMServices[i].OnEnd(currentState);
        }
    }

    public void ServicesOnAnimationEnd()
    {
        for (int i = 0; i < service_count; i++)
        {
            fSMServices[i].OnAnimationEnd(currentState);
        }
    }

    public void ServicesOnReStart()
    {
        for (int i = 0; i < service_count; i++)
        {
            fSMServices[i].ReStart(currentState);
        }
    }


    public void ServicesOnDisable()
    {
        for (int i = 0; i < service_count; i++)
        {
            fSMServices[i].OnDisable(currentState);
        }
    }




    #region 状态绑定的事件:进入的 更新时(每帧) 退出  当动作结束的时候

    //alt+enter

    //存储每个状态<不同类型的事件,<同一个类型可以存在多个事件,所以用了List来进行缓存>>
    //int 状态ID,Dictionary事件容器
    //事件容器StateEventType:事件类型 value(List<Action>) 代表类型对应的事件列表
    public Dictionary<int, Dictionary<StateEventType, List<Action>>> actions = new Dictionary<int, Dictionary<StateEventType, List<Action>>>();
    /// <summary>
    /// 添加事件的接口
    /// </summary>
    /// <param name="id">状态ID</param>
    /// <param name="t">事件类型</param>
    /// <param name="action">事件</param>
    public void AddListener(int id, StateEventType t, Action action)
    {
        if (!actions.ContainsKey(id))
        {
            actions[id] = new Dictionary<StateEventType, List<Action>>();
        }

        //如果不包含对应的事件类型 begin end
        if (actions[id].ContainsKey(t) == false)
        {
            //actions[id] = new Dictionary<StateEventType, List<Action>>();
            List<Action> list = new List<Action>();
            list.Add(action);
            actions[id][t] = list;
        }
        else
        {
            actions[id][t].Add(action);
        }
    }

    /// <summary>
    /// 提供快速调用不同状态不同类型的事件
    /// </summary>
    /// <param name="id">状态ID</param>
    /// <param name="t">事件类型</param>
    public void DOStateEvent(int id, StateEventType t)
    {
        if (actions.TryGetValue(id, out var v))
        {
            if (v.TryGetValue(t, out var lst))
            {
                for (int i = 0; i < lst.Count; i++)
                {
                    lst[i].Invoke();
                    if (currentState.id != id)
                    {
                        return;
                    }
                }
            }
        }
    }

    internal void AnimationOnPlayEnd()
    {
        var _id = currentState.id;

        DOStateEvent(currentState.id, StateEventType.onAnmEnd);
        ServicesOnAnimationEnd();

        if (currentState.id != _id)
        {
            return;
        }

        switch (currentState.excel_config.on_anm_end)
        {
            case -1:
                break;
            case 0:
                ServicesOnReStart();
                return;
            default:
                ToNext(currentState.excel_config.on_anm_end);
                break;
        }
    }

    bool ground_check = false;
    /// <summary>
    /// 移动
    /// </summary>
    /// <param name="d">移动的方向</param>
    /// <param name="transforDirection">是否需要根据自身局部坐标做转化</param>
    /// <param name="deltaTime"></param>
    /// <param name="_add_gravity"></param>
    /// <param name="_do_ground_check"></param>
    public void Move(Vector3 d, bool transforDirection, bool deltaTime = true, bool _add_gravity = true, bool _do_ground_check = true)
    {
        if (transforDirection)
        {
            d = this._transform.TransformDirection(d);
        }
        Vector3 d2;
        if (_add_gravity)
        {
            d2 = (d + GameDefine._Gravity) * (deltaTime ? Time.deltaTime : 1);
        }
        else
        {
            d2 = d * (deltaTime ? Time.deltaTime : 1);
        }
        characterController.Move(d2);
        if (_do_ground_check)
        {
            ground_check = true;
        }
    }

    /// <summary>
    /// 添加力
    /// </summary>
    /// <param name="speed">移动的向量</param>
    /// <param name="ignore_gravity">是否忽略重力</param>
    internal void AddForce(Vector3 speed, bool ignore_gravity)
    {
        //位移
        Move(speed, true, _add_gravity: ignore_gravity == false, _do_ground_check: !ignore_gravity);
    }

    internal void RemoveForce()
    {
       
    }



    Dictionary<string, GameObject> hangPoint = new Dictionary<string, GameObject>();
    internal GameObject GetHangPoint(string o_id)
    {
        if (string.IsNullOrEmpty(o_id))
        {
            return _gameObject;
        }

        if (hangPoint.TryGetValue(o_id, out var x))
        {
            return x;
        }
        var go = _transform.Find(o_id);
        if (go != null)
        {
            hangPoint[o_id] = go.gameObject;
            return go.gameObject;
        }
        else
        {
            hangPoint[o_id] = null;
            return null;
        }
    }

    FSM atk_fsm;//当前锁定的目标
    /// <summary>
    /// 返回当前锁定的单位
    /// </summary>
    /// <param name="spawn_point_type">0自身 1锁定的目标</param>
    /// <param name="spawn_hang_point">路径</param>
    /// <returns></returns>
    internal GameObject GetAtkTarget(int spawn_point_type, string spawn_hang_point)
    {
        if (spawn_point_type == 0)//0自身 1目标 挂点
        {
            return GetHangPoint(spawn_hang_point);
        }
        else if (spawn_point_type == 1)
        {
            if (atk_fsm != null)
            {
                return atk_fsm.GetHangPoint(spawn_hang_point);
            }
            else
            {
                return null;
            }
        }
        return null;
    }

    //拾取道具 升级状态 调用不同的特效配置
    internal void GetDrop()
    {
        var r = UnityEngine.Random.Range(1046, 1051);
        if (stateData.ContainsKey(r))
        {
            if (stateData[r].level < 4)
            {
                stateData[r].level += 1;
                //Debug.LogError($"升级的状态是:" + r);
                if (r == 1050)
                {
                    if (stateData[1051].level < 4)
                    {
                        stateData[1051].level += 1;
                    }
                }
            }
        }
    }

    internal void BeHit(float3 position, float3 float3)
    {
        
    }

    #endregion

}

public enum StateEventType
{
    begin,//开始进入
    update,//每帧更新
    end,//状态退出
    onAnmEnd,//当动作结束的时候
}

public class PlayerState
{
    public int id;//状态ID
    public PlayerStateEntity excel_config;//表格配置
    //动作通知事件
    public StateEntity stateEntity;//unity内的配置:特效 位移
    public float clipLength;//动作的时长
    public SkillEntity skill;//技能配置表
    public float begin;//进入状态的开始时间
    public int level;//级别
    public void SetBeginTime()
    {
        begin = Time.time;
    }

    public bool IsCD()
    {

        if (skill != null) { return false; }
        return Time.time - begin < skill.cd;
    }

    public string Info()
    {
        return $"状态:{id}_{excel_config.info}";
    }
}