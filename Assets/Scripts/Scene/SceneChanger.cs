using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviourSingletonTemplate<SceneChanger>
{
    Scene m_CurrentSceneData;
    public Scene CurrentSceneData => m_CurrentSceneData;

    bool m_IsChangingScene = false;
    string m_ChangeTargetSceneName;
    string m_ChangingSceneName;

    float m_SceneChangeProgress = 0f;


    protected override void Awake()
    {
        base.Awake();
    }

    public void ChangeScene(string sceneName)
    {
        if (m_CurrentSceneData.IsValid() == true && m_CurrentSceneData.name.Equals(sceneName))
        {
            Debug.LogError("same scene");
            return;
        }

        m_ChangeTargetSceneName = sceneName;

        if (m_IsChangingScene == false)
        {
            StartCoroutine(ChangeSceneRoutine());
        }
    }

    public IEnumerator ChangeSceneRoutine()
    {
        m_IsChangingScene = true;
        AsyncOperation async = null;
        while (m_ChangeTargetSceneName != m_CurrentSceneData.name)
        {
            // 빈 Scene을 호출하여 깔끔하게 다 날려버린다.
            async = SceneManager.LoadSceneAsync("Empty", LoadSceneMode.Single);
            while(async.isDone == false)
            {
                m_SceneChangeProgress = 0.0f + async.progress * 0.2f;
                yield return null;
            }
            m_SceneChangeProgress = 0.2f;
            
            // 사용되지 않는 리소스들 언로드
            async = Resources.UnloadUnusedAssets();
            while(async.isDone == false)
            {
                m_SceneChangeProgress = 0.2f + async.progress * 0.2f;
                yield return null;
            }
            m_SceneChangeProgress = 0.4f;

            // 혹시 모르니 GC도 한번..
            System.GC.Collect();

            // 본격적으로 씬 로드
            m_ChangingSceneName = m_ChangeTargetSceneName;
            async = SceneManager.LoadSceneAsync(m_ChangingSceneName, LoadSceneMode.Single);
            while(async.isDone == false)
            {
                m_SceneChangeProgress = 0.4f + async.progress * 0.6f;
                yield return null;
            }
            m_SceneChangeProgress = 1.0f;

            // 완료!
            m_CurrentSceneData = SceneManager.GetSceneByName(m_ChangingSceneName);
        }
        m_IsChangingScene = false;
        m_ChangingSceneName = string.Empty;
        m_ChangeTargetSceneName = string.Empty;
    }
}
