using System.Collections.Generic;

using UnityEngine;

public class UnitStateManager
{
    static UnitStateManager instance = new UnitStateManager();
    public static UnitStateManager Instance => instance;

    public Dictionary<int, Dictionary<int, StateEntity>> states = new Dictionary<int, Dictionary<int, StateEntity>>();

    public Dictionary<int, StateScriptableObject> unit_state = new Dictionary<int, StateScriptableObject>();

    /// <summary>
    /// 获取配置
    /// </summary>
    /// <param name="unit">单位ID</param>
    /// <param name="state">状态ID</param>
    /// <returns></returns>
    public StateEntity Get(int unit, int state)
    {
        if (states.ContainsKey(unit) == false)
        {
            var state_config = Resources.Load<StateScriptableObject>($"StateConfig/{unit}");
            unit_state[unit] = state_config;

            var _state_dct = new Dictionary<int, StateEntity>();
            foreach (var item in state_config.states)
            {
                _state_dct[item.id] = item;
            }
            states[unit] = _state_dct;
        }
        return states[unit][state];
    }
}
