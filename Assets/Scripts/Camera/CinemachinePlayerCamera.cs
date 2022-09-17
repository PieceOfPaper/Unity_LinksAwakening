using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineVirtualCameraBase))]
public class CinemachinePlayerCamera : MonoBehaviour
{
    CinemachineVirtualCameraBase m_VCam;
    public CinemachineVirtualCameraBase VirtualCamera
    {
        get
        {
            if (m_VCam == null)
            {
                m_VCam = GetComponent<CinemachineVirtualCameraBase>();
            }
            return m_VCam;
        }
    }

    PlayerEntityController m_PrevPlayerCtrler = null;
    

    private void OnEnable()
    {
        Refresh();
    }

    private void Start()
    {
        Refresh();
    }

    private void Update()
    {
        if (m_PrevPlayerCtrler != GamePlayData.PlayerCtrler)
        {
            Refresh();
        }
    }

    private void LateUpdate()
    {
        if (m_PrevPlayerCtrler != GamePlayData.PlayerCtrler)
        {
            Refresh();
        }
    }

    public void Refresh()
    {
        if (GamePlayData.PlayerCtrler == null)
        {
            VirtualCamera.Follow = null;
            VirtualCamera.LookAt = null;
        }
        else
        {
            VirtualCamera.Follow = GamePlayData.PlayerCtrler.transform;
            VirtualCamera.LookAt = GamePlayData.PlayerCtrler.transform;
        }
        m_PrevPlayerCtrler = GamePlayData.PlayerCtrler;
    }
}
