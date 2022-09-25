using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTrigger_PlayerEntityWarp : SceneTriggerTemplate<PlayerEntityController>
{
    [SerializeField] string WarpSceneName;

    protected override void OnEnterTarget(PlayerEntityController entity)
    {
        base.OnEnterTarget(entity);
        
        SceneChanger.Instance.ChangeScene(WarpSceneName);
    }
}
