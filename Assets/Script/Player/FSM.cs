using System;
using System.Collections.Generic;
using Game.Config;
using RootMotion.FinalIK;

using Unity.Mathematics;

using UnityEngine;

public class FSM : MonoBehaviour
{
    public bool AI;
    public int id;//��λ��ID
    public UnitEntity unitEntity;//��λ������

    public PlayerState currentState;
    Dictionary<int, PlayerState> stateData = new Dictionary<int, PlayerState>();

    public Animator animator;
    public UnityEngine.CharacterController characterController;


    public UnitAttEntity att_base;//������
    public UnitAttEntity att_crn;//��ǰ����==>����ֵ


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

        //���еĶ������� 
        var clips = animator.runtimeAnimatorController.animationClips;
        Dictionary<string, float> clipLength = new Dictionary<string, float>();
        foreach (var clip in clips)
        {
            clipLength[clip.name] = clip.length;
        }

        //����EXCEL
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

        //�󶨵��¼�: (�ո�)λ��   (������)�չ� (wasd)�ƶ� (1234)�����ֵ���:ը�� ���� �ɵ� ����  wasḑ���ʱ��,ֹͣ�ƶ�
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

            //Mathf.Atan2 ���к��� �󻡶� * Mathf.Rad2Deg(����ת����) >> ����

            //��
            //��һ:���������ĽǶ�
            //�ڶ�:���ϵ�ǰ�����Y����ת����
            //����:�õ�Ŀ�곯��ĽǶ�
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;// +GameDefine._Camera.eulerAngles.y;

            //��һ����ֵ�˶�
            //float rotation = Mathf.SmoothDampAngle(_transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
            //    RotationSmoothTime);

            float y = Mathf.LerpAngle(transform.eulerAngles.y, /*rotation*/_targetRotation, Time.deltaTime * 180);
            this.transform.eulerAngles = new Vector3(0, y, 0);

            //��ɫ����ת��Ŀ��Ƕ�ȥ
            // rotate to face input direction relative to camera position
            //_transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

            //����Ŀ�귽�� ͨ������Ƕ�
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
    /// ״̬�л���ϵ�ļ��
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
            //������ǰҡ���� �����Ѿ������ҡ�׶��� �Ǳ�ʾ���Խ����л�
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
        //����Ӳ��������
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
                DOStateEvent(currentState.id, StateEventType.update);//״̬ÿִ֡�е��¼�
            }

            //ToGround();
        }
    }

    #region ״̬�л�

    #endregion
    public bool ToNext(int next)
    {
        if (stateData.ContainsKey(next))
        {
            //if (currentState != null)
            //{
            //    Debug.Log($"{this.gameObject.name}:�л�״̬:{stateData[next].Info()}  ��ǰ��:{currentState.Info()}");
            //}
            //else
            //{
            //    Debug.Log($"{this.gameObject.name}:�л�״̬:{stateData[next].Info()}");
            //}

            //��һ��״̬ �Ƿ񻹴���CD��
            var next_state = stateData[next];
            //if (next_state.skill != null && next_state.begin != 0 && GameTime.time - next_state.begin < next_state.skill.cd)
            //{
            //    MainViewController.Instance.OpenCD_Tips();
            //    return false;
            //}

            if (currentState != null)
            {
                DOStateEvent(currentState.id, StateEventType.end);//״̬�󶨵��˳��¼�
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
            DOStateEvent(currentState.id, StateEventType.begin); //ִ�е�ǰ״̬�Ŀ�ʼ(����)�¼�
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

    //ע�� ��Ӳ�ͬ�������
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




    #region ״̬�󶨵��¼�:����� ����ʱ(ÿ֡) �˳�  ������������ʱ��

    //alt+enter

    //�洢ÿ��״̬<��ͬ���͵��¼�,<ͬһ�����Ϳ��Դ��ڶ���¼�,��������List�����л���>>
    //int ״̬ID,Dictionary�¼�����
    //�¼�����StateEventType:�¼����� value(List<Action>) �������Ͷ�Ӧ���¼��б�
    public Dictionary<int, Dictionary<StateEventType, List<Action>>> actions = new Dictionary<int, Dictionary<StateEventType, List<Action>>>();
    /// <summary>
    /// ����¼��Ľӿ�
    /// </summary>
    /// <param name="id">״̬ID</param>
    /// <param name="t">�¼�����</param>
    /// <param name="action">�¼�</param>
    public void AddListener(int id, StateEventType t, Action action)
    {
        if (!actions.ContainsKey(id))
        {
            actions[id] = new Dictionary<StateEventType, List<Action>>();
        }

        //�����������Ӧ���¼����� begin end
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
    /// �ṩ���ٵ��ò�ͬ״̬��ͬ���͵��¼�
    /// </summary>
    /// <param name="id">״̬ID</param>
    /// <param name="t">�¼�����</param>
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
    /// �ƶ�
    /// </summary>
    /// <param name="d">�ƶ��ķ���</param>
    /// <param name="transforDirection">�Ƿ���Ҫ��������ֲ�������ת��</param>
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
    /// �����
    /// </summary>
    /// <param name="speed">�ƶ�������</param>
    /// <param name="ignore_gravity">�Ƿ��������</param>
    internal void AddForce(Vector3 speed, bool ignore_gravity)
    {
        //λ��
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

    FSM atk_fsm;//��ǰ������Ŀ��
    /// <summary>
    /// ���ص�ǰ�����ĵ�λ
    /// </summary>
    /// <param name="spawn_point_type">0���� 1������Ŀ��</param>
    /// <param name="spawn_hang_point">·��</param>
    /// <returns></returns>
    internal GameObject GetAtkTarget(int spawn_point_type, string spawn_hang_point)
    {
        if (spawn_point_type == 0)//0���� 1Ŀ�� �ҵ�
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

    //ʰȡ���� ����״̬ ���ò�ͬ����Ч����
    internal void GetDrop()
    {
        var r = UnityEngine.Random.Range(1046, 1051);
        if (stateData.ContainsKey(r))
        {
            if (stateData[r].level < 4)
            {
                stateData[r].level += 1;
                //Debug.LogError($"������״̬��:" + r);
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
    begin,//��ʼ����
    update,//ÿ֡����
    end,//״̬�˳�
    onAnmEnd,//������������ʱ��
}

public class PlayerState
{
    public int id;//״̬ID
    public PlayerStateEntity excel_config;//�������
    //����֪ͨ�¼�
    public StateEntity stateEntity;//unity�ڵ�����:��Ч λ��
    public float clipLength;//������ʱ��
    public SkillEntity skill;//�������ñ�
    public float begin;//����״̬�Ŀ�ʼʱ��
    public int level;//����
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
        return $"״̬:{id}_{excel_config.info}";
    }
}