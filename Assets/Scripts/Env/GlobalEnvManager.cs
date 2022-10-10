using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalEnvManager : MonoBehaviourSingletonTemplate<GlobalEnvManager>
{
    Light m_GlobalDirectionalLight;

    private void InitGlobalDirectionalLight()
    {
        var obj = new GameObject("GlobalDirectionalLight");
        obj.transform.SetParent(transform);
        m_GlobalDirectionalLight = obj.AddComponent<Light>();
        m_GlobalDirectionalLight.type = LightType.Directional;
        m_GlobalDirectionalLight.renderMode = LightRenderMode.Auto;
        m_GlobalDirectionalLight.lightmapBakeType = LightmapBakeType.Mixed;
    }

    public void ChangeGlobalDirectionalLight(Light light)
    {
        if (m_GlobalDirectionalLight == null) InitGlobalDirectionalLight();
        
        m_GlobalDirectionalLight.transform.position = light.transform.position;
        m_GlobalDirectionalLight.transform.rotation = light.transform.rotation;
        
        m_GlobalDirectionalLight.color = light.color;
        m_GlobalDirectionalLight.useColorTemperature = light.useColorTemperature;
        m_GlobalDirectionalLight.colorTemperature = light.colorTemperature;
        m_GlobalDirectionalLight.range = light.range;
        m_GlobalDirectionalLight.intensity = light.intensity;
        m_GlobalDirectionalLight.bounceIntensity = light.bounceIntensity;

        m_GlobalDirectionalLight.flare = light.flare;
        m_GlobalDirectionalLight.cullingMask = light.cullingMask;
        m_GlobalDirectionalLight.renderingLayerMask = light.renderingLayerMask;

        m_GlobalDirectionalLight.useShadowMatrixOverride = light.useShadowMatrixOverride;
        m_GlobalDirectionalLight.lightShadowCasterMode = light.lightShadowCasterMode;
        m_GlobalDirectionalLight.shadowRadius = light.shadowRadius;
        m_GlobalDirectionalLight.shadowAngle = light.shadowAngle;
        m_GlobalDirectionalLight.shadows = light.shadows;
        m_GlobalDirectionalLight.shadowStrength = light.shadowStrength;
        m_GlobalDirectionalLight.shadowResolution = light.shadowResolution;
        m_GlobalDirectionalLight.layerShadowCullDistances = light.layerShadowCullDistances;
        m_GlobalDirectionalLight.shadowMatrixOverride = light.shadowMatrixOverride;
        m_GlobalDirectionalLight.shadowNearPlane = light.shadowNearPlane;
        m_GlobalDirectionalLight.useViewFrustumForShadowCasterCull = light.useViewFrustumForShadowCasterCull;
        m_GlobalDirectionalLight.shadowCustomResolution = light.shadowCustomResolution;
        m_GlobalDirectionalLight.shadowBias = light.shadowBias;
        m_GlobalDirectionalLight.shadowNormalBias = light.shadowNormalBias;

        m_GlobalDirectionalLight.cookieSize = light.cookieSize;
        m_GlobalDirectionalLight.cookie = light.cookie;
        m_GlobalDirectionalLight.areaSize = light.areaSize;
        m_GlobalDirectionalLight.shape = light.shape;
        m_GlobalDirectionalLight.innerSpotAngle = light.innerSpotAngle;
        m_GlobalDirectionalLight.spotAngle = light.spotAngle;
        m_GlobalDirectionalLight.useBoundingSphereOverride = light.useBoundingSphereOverride;
        m_GlobalDirectionalLight.boundingSphereOverride = light.boundingSphereOverride;
    }


    UnityEngine.Rendering.Volume m_GlobalVolume;

    private void InitGlobalVolume()
    {
        var obj = new GameObject("GlobalVolume");
        obj.transform.SetParent(transform);
        m_GlobalVolume = obj.AddComponent<UnityEngine.Rendering.Volume>();
    }

    public void ChangeGlobalVolume(UnityEngine.Rendering.VolumeProfile profile)
    {
        if (m_GlobalVolume == null) InitGlobalVolume();
        m_GlobalVolume.sharedProfile = profile;
    }


    Cinemachine.CinemachineVirtualCameraBase m_CurrentVirtualCamera;

    public void ChangeVirtualCamera(Cinemachine.CinemachineVirtualCameraBase vcam)
    {
        if (m_CurrentVirtualCamera  != null) m_CurrentVirtualCamera.Priority = 10;
        m_CurrentVirtualCamera = vcam;
        if (m_CurrentVirtualCamera != null) m_CurrentVirtualCamera.Priority = 11;
    }
}
