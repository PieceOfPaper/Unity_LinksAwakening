using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTrigger_PlayerEntityChangeEnv : SceneTriggerTemplate<PlayerEntityController>
{
    [SerializeField] Light m_DirectionalLight;
    [SerializeField] UnityEngine.Rendering.Volume m_Volume;
    [SerializeField] Cinemachine.CinemachineVirtualCamera m_VirtualCamera;

    private void Awake()
    {
        if (m_DirectionalLight != null) m_DirectionalLight.enabled = false;
        if (m_Volume != null) m_Volume.enabled = false;
        // if (m_VirtualCamera != null) m_VirtualCamera.enabled = false;
    }

    protected override void OnEnterTarget(PlayerEntityController entity)
    {
        base.OnEnterTarget(entity);

        if (m_DirectionalLight != null) GlobalEnvManager.Instance.ChangeGlobalDirectionalLight(m_DirectionalLight);
        if (m_Volume != null) GlobalEnvManager.Instance.ChangeGlobalVolume(m_Volume.profile);
        if (m_VirtualCamera != null) GlobalEnvManager.Instance.ChangeVirtualCamera(m_VirtualCamera);
    }
}
