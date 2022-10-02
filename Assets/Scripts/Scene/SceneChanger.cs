using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviourSingletonTemplate<SceneChanger>
{
    Scene m_CurrentSceneData;
    public Scene CurrentSceneData => m_CurrentSceneData;

    SubSceneSetting m_CurrentSubSceneSetting;


    Vector3 PlayerPosition
    {
        get
        {
            if (GamePlayData.PlayerCtrler == null) return Vector3.zero;
            return GamePlayData.PlayerCtrler.transform.position;
        }
        set
        {
            if (GamePlayData.PlayerCtrler == null) return;
            GamePlayData.PlayerCtrler.transform.position = value;
        }
    }


    bool m_IsChangingScene = false;
    string m_ChangeTargetSceneName;
    Vector3 m_ChangeTargetPosition;
    string m_ChangingSceneName;

    float m_SceneChangeProgress = 0f;


    List<string> m_LoadedSubSceneNameList = new List<string>();

    private struct SubSceneLoadForm
    {
        public bool IsLoad;
        public string SceneName;
    }
    bool m_IsProcessingSubSceneLoadRoutine = false;
    List<SubSceneLoadForm> m_SubSceneLoadFormList = new List<SubSceneLoadForm>();


    protected override void Awake()
    {
        base.Awake();
    }

    private void LateUpdate()
    {
        CheckSubSceneLoad();
    }

    public void ChangeScene(string sceneName, Vector3 pos)
    {
        if (m_CurrentSceneData.IsValid() == true && m_CurrentSceneData.name.Equals(sceneName))
        {
            Debug.LogError("same scene");
            return;
        }

        m_ChangeTargetSceneName = sceneName;
        m_ChangeTargetPosition = pos;

        if (m_IsChangingScene == false)
        {
            StartCoroutine(ChangeSceneRoutine());
        }
    }

    private IEnumerator ChangeSceneRoutine()
    {
        m_IsChangingScene = true;
        AsyncOperation async = null;
        SubSceneSetting subSceneSetting = null;
        while (m_ChangeTargetSceneName != m_CurrentSceneData.name)
        {
            m_LoadedSubSceneNameList.Clear();
            m_SubSceneLoadFormList.Clear();
            m_IsProcessingSubSceneLoadRoutine = false;
            StopCoroutine("ProcessSubSceneLoadRoutine");

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
            subSceneSetting = Resources.Load<SubSceneSetting>($"SubSceneSettings/{m_ChangingSceneName}");
            async = SceneManager.LoadSceneAsync(m_ChangingSceneName, LoadSceneMode.Single);
            while(async.isDone == false)
            {
                if (subSceneSetting == null)
                    m_SceneChangeProgress = 0.4f + async.progress * 0.6f;
                else 
                    m_SceneChangeProgress = 0.4f + async.progress * 0.2f;
                yield return null;
            }

            if (subSceneSetting == null)
            {
                m_SceneChangeProgress = 1.0f;
            }
            else
            {
                //서브씬 로드
                m_SceneChangeProgress = 0.6f;
                var subScenes = subSceneSetting.SubSceneDatList.FindAll(m => m.IsContainIn(Camera.main, PlayerPosition));
                float subSceneProgress = (1.0f / subScenes.Count) * 0.4f;
                for (int i = 0; i < subScenes.Count; i ++)
                {
                    async = SceneManager.LoadSceneAsync(subScenes[i].sceneName, LoadSceneMode.Additive);
                    while(async.isDone == false)
                    {
                        m_SceneChangeProgress = 0.6f + (subSceneProgress * i) + async.progress * subSceneProgress;
                        yield return null;
                    }
                    m_LoadedSubSceneNameList.Add(subScenes[i].sceneName);
                }
                m_SceneChangeProgress = 1.0f;
            }

            // 완료!
            m_CurrentSceneData = SceneManager.GetSceneByName(m_ChangingSceneName);
            m_CurrentSubSceneSetting = subSceneSetting;
        }

        PlayerPosition = m_ChangeTargetPosition;

        m_IsChangingScene = false;
        m_ChangingSceneName = string.Empty;
        m_ChangeTargetSceneName = string.Empty;
    }

    public void CheckSubSceneLoad()
    {
        if (m_IsChangingScene == true)
            return;

        if (m_CurrentSubSceneSetting == null || m_CurrentSubSceneSetting.SubSceneDatList.Count == 0)
            return;

        //영역 밖으로 벗어난 서브씬 체크
        for (int i = 0; i < m_LoadedSubSceneNameList.Count; i ++)
        {
            var subSceneData = m_CurrentSubSceneSetting.GetSubSceneDataBySceneName(m_LoadedSubSceneNameList[i]);
            if (subSceneData.IsContainOut(Camera.main, PlayerPosition))
            {
                var index = m_SubSceneLoadFormList.FindIndex(m => m.SceneName == m_LoadedSubSceneNameList[i]);
                if (index >= 0)
                {
                    if (m_SubSceneLoadFormList[index].IsLoad == true)
                    {
                        m_SubSceneLoadFormList.RemoveAt(index);
                    }
                }
                m_SubSceneLoadFormList.Add(new SubSceneLoadForm() { IsLoad = false, SceneName = m_LoadedSubSceneNameList[i]});
            }
        }

        //역역 안으로 들어온 서브씬 체크
        foreach (var subSceneData in m_CurrentSubSceneSetting.SubSceneDatList)
        {
            if (m_LoadedSubSceneNameList.Contains(subSceneData.sceneName) == true) continue;

            var index = m_SubSceneLoadFormList.FindIndex(m => m.SceneName == subSceneData.sceneName);
            if (index >= 0)
            {
                if (m_SubSceneLoadFormList[index].IsLoad == true)
                    continue;
            }

            if (subSceneData.IsContainIn(Camera.main, PlayerPosition))
            {
                if (index >= 0)
                {
                    if (m_SubSceneLoadFormList[index].IsLoad == false)
                    {
                        m_SubSceneLoadFormList.RemoveAt(index);
                    }
                }
                m_SubSceneLoadFormList.Add(new SubSceneLoadForm() { IsLoad = true, SceneName = subSceneData.sceneName});
            }
        }

        if (m_IsProcessingSubSceneLoadRoutine == true)
            return;

        StartCoroutine(ProcessSubSceneLoadRoutine());
    }

    private IEnumerator ProcessSubSceneLoadRoutine()
    {
        m_IsProcessingSubSceneLoadRoutine = true;
        while (m_SubSceneLoadFormList.Count > 0)
        {
            var form = m_SubSceneLoadFormList[0];
            m_SubSceneLoadFormList.RemoveAt(0);
            if (form.IsLoad == true)
            {
                m_LoadedSubSceneNameList.Add(form.SceneName);
                yield return SceneManager.LoadSceneAsync(form.SceneName, LoadSceneMode.Additive);
            }
            else
            {
                m_LoadedSubSceneNameList.Remove(form.SceneName);
                yield return SceneManager.UnloadSceneAsync(form.SceneName);
            }
        }
        m_IsProcessingSubSceneLoadRoutine = false;
    }
}
