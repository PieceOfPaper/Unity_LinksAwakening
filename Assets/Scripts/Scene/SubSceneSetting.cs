using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SubSceneSetting : ScriptableObject
{
    [SerializeField] public List<SubSceneData> SubSceneDatList = new List<SubSceneData>();

    public const float SUBSCENE_CHECK_POS_RANGE_FACTOR = 1.5f;
    static readonly Rect SUBSCENE_CHECK_VIEWPORT_RECT = new Rect(0, 0, 1, 1);

    [System.Serializable]
    public struct SubSceneData
    {
        [SerializeField] public string sceneName;
        [SerializeField] public Vector3 center;
        [SerializeField] public float range;

        public bool IsContainIn(Camera cam, Vector3 playerPos)
        {
            playerPos.y = 0f;

            // 영역 내에 플레이어가 들어왔을 때는 확실한 상태.
            if (Vector3.SqrMagnitude(playerPos - center) < range * range)
                return true;

            // 영역 밖인데 짱멀리 있을 때는 고려하지 않는다.
            if (Vector3.SqrMagnitude(playerPos - center) >= (range * SUBSCENE_CHECK_POS_RANGE_FACTOR) * (range * SUBSCENE_CHECK_POS_RANGE_FACTOR))
                return false;

            // 카메라와 가장 가까운 포지션을 구해서, 카메라에 그려지는 지 WorldToViewportPoint로 체크
            if (cam == null) return false;

            var angle = 90 - Mathf.Atan2(cam.transform.position.z - center.z, cam.transform.position.x - center.x) * Mathf.Rad2Deg;
            float[] checkAngles = new float[]{
                angle, //카메라와 가장 가까운 각도
                angle - 90, //카메라 기준 옆
                angle + 90, //카메라 기준 옆
            };

            for (int i = 0 ; i < checkAngles.Length; i ++)
            {
                var checkPos = center + Quaternion.Euler(0, checkAngles[i], 0) * Vector3.forward * range;
                var viewportPoint = cam.WorldToViewportPoint(checkPos);
                if (SUBSCENE_CHECK_VIEWPORT_RECT.Contains(viewportPoint) == true)
                    return true;
            }
            return false;
        }

        public bool IsContainOut(Camera cam, Vector3 playerPos)
        {
            playerPos.y = 0f;

            // 영역 밖인데 짱멀리 있을 때.
            if (Vector3.SqrMagnitude(playerPos - center) >= (range * SUBSCENE_CHECK_POS_RANGE_FACTOR) * (range * SUBSCENE_CHECK_POS_RANGE_FACTOR))
                return true;
            
            return false;
        }

        
#if UNITY_EDITOR
        public void Bake()
        {
            Scene sceneData = default;
            bool isLoadedScene = false;
            for (int i = 0; i < UnityEditor.SceneManagement.EditorSceneManager.sceneCount; i ++)
            {
                sceneData = UnityEditor.SceneManagement.EditorSceneManager.GetSceneAt(i);
                if (sceneData.name == sceneName)
                {
                    isLoadedScene = sceneData.isLoaded;
                    break;
                }
            }

            // 서브 씬 로드
            if (isLoadedScene == false)
            {
                sceneData = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(SceneUtility.GetEditorBuildSettingsScenePathBySceneName(sceneName), UnityEditor.SceneManagement.OpenSceneMode.Additive);
            }
            
            if (sceneData.IsValid() == false)
            {
                center = Vector3.zero;
                range = 0f;
                if (isLoadedScene == false)
                    UnityEditor.SceneManagement.EditorSceneManager.CloseScene(sceneData, true);
                return;
            }

            // 서브씬에 있는 모든 Renderer 검색
            List<Renderer> rendererList = new List<Renderer>();
            var rootObjs = sceneData.GetRootGameObjects();
            for (int i = 0; i < rootObjs.Length; i ++)
            {
                if (rootObjs[i] == null) continue;

                var renderers = rootObjs[i].GetComponentsInChildren<Renderer>();
                if (renderers == null || renderers.Length == 0) continue;

                rendererList.AddRange(renderers);
            }

            // MeshRenderer에서 각 Mesh의 Vertex들의 World좌표들을 가져옴.
            List<Vector3> pointList = new List<Vector3>();
            foreach (var renderer in rendererList)
            {
                if (renderer == null) continue;
                if (renderer.enabled == false) continue;
                if (renderer.transform == null) continue;
                // if (meshRenderer.gameObject.isStatic == false) continue;

                Mesh mesh = null;
                if (renderer is MeshRenderer)
                {
                    var meshFilter = renderer.GetComponent<MeshFilter>();
                    if (meshFilter == null) continue;

                    mesh = meshFilter.sharedMesh;
                }
                else if (renderer is SkinnedMeshRenderer skinnedMeshRenderer)
                {
                    mesh = skinnedMeshRenderer.sharedMesh;
                }
                
                if (mesh == null) continue;

                var vertices = new List<Vector3>();
                mesh.GetVertices(vertices);

                foreach (var vertex in vertices)
                {
                    var point = renderer.transform.TransformPoint(vertex);
                    point.y = 0f; //쿼터뷰 게임이라 y를 계산하는 것 자체가 사치.
                    pointList.Add(point);
                }
            }

            //중심 및 거리 게산
            center = Vector3.zero;
            foreach (var point in pointList)
            {
                center += point;
            }
            center /= pointList.Count;

            range = 0f;
            foreach (var point in pointList)
            {
                if (Vector3.SqrMagnitude(point - center) > range * range)
                {
                    range = Vector3.Distance(point, center);
                }
            }
            
            // 서브씬 언로드
            if (isLoadedScene == false)
                UnityEditor.SceneManagement.EditorSceneManager.CloseScene(sceneData, true);
        }
#endif
    }


    Dictionary<string, SubSceneData> m_DicSubSceneData = null;

    public SubSceneData GetSubSceneDataBySceneName(string sceneName)
    {
#if UNITY_EDITOR
        if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode == false)
            return SubSceneDatList.Find(m => m.sceneName == sceneName);
#endif
        if (m_DicSubSceneData == null)
        {
            m_DicSubSceneData = new Dictionary<string, SubSceneData>();
            for (int i = 0; i < SubSceneDatList.Count; i ++)
            {
                if (m_DicSubSceneData.ContainsKey(SubSceneDatList[i].sceneName))
                    continue;

                m_DicSubSceneData.Add(SubSceneDatList[i].sceneName, SubSceneDatList[i]);
            }
        }

        if (m_DicSubSceneData.ContainsKey(sceneName))
            return m_DicSubSceneData[sceneName];

        return default;
    }



#if UNITY_EDITOR
    public static SubSceneSetting GenerateSubSceneSetting(string assetName)
    {
        SubSceneSetting asset = ScriptableObject.CreateInstance<SubSceneSetting>();

        UnityEditor.AssetDatabase.CreateAsset(asset, $"Assets/Resources/SubSceneSettings/{assetName}.asset");
        UnityEditor.AssetDatabase.SaveAssets();

        return asset;
    }

    [UnityEditor.MenuItem("Assets/Generate SubSceneSetting")]
    private static void MenuItem_GenerateSubSceneSetting()
    {
        if (UnityEditor.Selection.objects == null)
            return;

        List<Object> assetObjList = new List<Object>();
        foreach (var obj in UnityEditor.Selection.objects)
        {
            if (obj is UnityEditor.SceneAsset sceneAsset)
            {
                var newAsset = GenerateSubSceneSetting(sceneAsset.name);
                assetObjList.Add(newAsset);
            }
        }
        if (assetObjList.Count > 0)
        {
            UnityEditor.EditorUtility.FocusProjectWindow();
            UnityEditor.Selection.objects = assetObjList.ToArray();
        }
    }

    [UnityEditor.MenuItem("Assets/Generate SubSceneSetting", true)]
    private static bool MenuItem_GenerateSubSceneSettingValidation()
    {
        if (UnityEditor.Selection.objects == null)
            return false;

        foreach (var obj in UnityEditor.Selection.objects)
        {
            if (obj is UnityEditor.SceneAsset) return true;
        }
        return false;
    }
#endif
}
