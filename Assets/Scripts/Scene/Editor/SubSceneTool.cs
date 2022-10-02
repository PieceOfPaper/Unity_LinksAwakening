using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

public class SubSceneTool : EditorWindow
{
    [MenuItem("Tools/SubScene Tool")]
    public static void Open()
    {
        var window = (SubSceneTool)EditorWindow.GetWindow(typeof(SubSceneTool), false, "SubScene Tool", true);
        window.Show();
    }


    public bool IsAutoSelectMainScene
    {
        set { EditorPrefs.SetBool("SubSceneTool_IsAutoSelectMainScene", value); }
        get { return EditorPrefs.GetBool("SubSceneTool_IsAutoSelectMainScene", true); }
    }

    public SceneAsset SelectedMainScene = null;
    public SubSceneSetting SelectedSubSceneSetting = null;
    public Vector2 SubSceneScoll = Vector2.zero;

    private void OnEnable()
    {
        EditorSceneManager.sceneOpened += OnSceneOpened;
    }

    private void OnDisable()
    {
        EditorSceneManager.sceneOpened -= OnSceneOpened;
    }
    
    private void OnFocus()
    {
        SceneView.duringSceneGui -= OnDuringSceneGUI;
        SceneView.duringSceneGui += OnDuringSceneGUI;
    }
    
    private void OnDestroy()
    {
        SceneView.duringSceneGui -= OnDuringSceneGUI;
    }

    private void OnSceneOpened(Scene scene, OpenSceneMode mode)
    {
        if (IsAutoSelectMainScene == true && mode == OpenSceneMode.Single)
            AutoSelectMainScene();
    }
    
    private void OnDuringSceneGUI(SceneView sceneView)
    {
        using (new Handles.DrawingScope())
        {
            if (SelectedSubSceneSetting != null)
            {
                foreach (var subSceneData in SelectedSubSceneSetting.SubSceneDatList)
                {
                    Handles.color = new Color(1f, 1f, 0f, 1.0f);
                    Handles.DrawWireDisc(subSceneData.center, Vector3.up, subSceneData.range);

                    if (Camera.main != null)
                    {
                        Handles.color = new Color(1f, 1f, 0f, 1.0f);
                        var angle = 90 - Mathf.Atan2(Camera.main.transform.position.z - subSceneData.center.z, Camera.main.transform.position.x - subSceneData.center.x) * Mathf.Rad2Deg;
                        
                        var checkPos = subSceneData.center + Quaternion.Euler(0, angle, 0) * Vector3.forward * subSceneData.range;
                        var viewportPoint = Camera.main.WorldToViewportPoint(checkPos);
                        Handles.DrawLine(subSceneData.center, checkPos, 1.0f);
                    }


                    Handles.color = new Color(1f, 1f, 0f, 0.25f);
                    Handles.DrawWireDisc(subSceneData.center, Vector3.up, subSceneData.range * SubSceneSetting.SUBSCENE_CHECK_POS_RANGE_OUT_FACTOR);
                }
            }
        }  
    }



    private void OnGUI()
    {
        var isAutoSelectMainScene = EditorGUILayout.Toggle("Auto Select", IsAutoSelectMainScene);
        if (isAutoSelectMainScene != IsAutoSelectMainScene)
        {
            IsAutoSelectMainScene = isAutoSelectMainScene;
            if (IsAutoSelectMainScene)
                AutoSelectMainScene();
        }
        using (new EditorGUI.DisabledScope(IsAutoSelectMainScene == true))
        {
            var selectedMainScene = (SceneAsset)EditorGUILayout.ObjectField("Main Scene", SelectedMainScene, typeof(SceneAsset), false);
            if (selectedMainScene != SelectedMainScene)
            {
                SelectedMainScene = selectedMainScene;
                OnChangeSelectedMainScene();
            }
        }

        if (SelectedSubSceneSetting == null)
        {
            using (new EditorGUI.DisabledScope(SelectedMainScene == null))
            {
                if (GUILayout.Button("Create Setting"))
                {
                    var setting = SubSceneSetting.GenerateSubSceneSetting(SelectedMainScene.name);
                    SelectedSubSceneSetting = setting;
                }
            }
        }
        else
        {
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.ObjectField("Setting", SelectedSubSceneSetting, typeof(SubSceneSetting), false);
            }
        }
        EditorGUILayout.Space();

        SubSceneScoll = EditorGUILayout.BeginScrollView(SubSceneScoll);
        OnGUI_SubScenes();
        EditorGUILayout.EndScrollView();
    }

    private void OnGUI_SubScenes()
    {
        if (SelectedSubSceneSetting == null)
            return;

        if (SelectedSubSceneSetting.SubSceneDatList == null)
            SelectedSubSceneSetting.SubSceneDatList = new List<SubSceneSetting.SubSceneData>();
        for (int i = 0; i < SelectedSubSceneSetting.SubSceneDatList.Count; i ++)
        {
            var data = SelectedSubSceneSetting.SubSceneDatList[i];
            var sceneData = SceneUtility.GetEditorOpenedSceneBySceneName(data.sceneName);

            EditorGUILayout.BeginVertical(GUI.skin.box);
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label($"[{i}]", GUILayout.ExpandWidth(false));
                data.sceneName = EditorGUILayout.TextField(data.sceneName, GUILayout.ExpandWidth(true));
                if (sceneData.IsValid() == true && sceneData.isLoaded == true)
                {
                    if (GUILayout.Button("Close", GUILayout.ExpandWidth(false)))
                    {
                        UnityEditor.SceneManagement.EditorSceneManager.CloseScene(sceneData, false);
                    }
                }
                else
                {
                    if (GUILayout.Button("Open", GUILayout.ExpandWidth(false)))
                    {
                        var path = SceneUtility.GetEditorBuildSettingsScenePathBySceneName(data.sceneName);
                        UnityEditor.SceneManagement.EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
                    }
                }
                if (GUILayout.Button("Bake", GUILayout.ExpandWidth(false)))
                {
                    data.Bake();
                }
                if (GUILayout.Button("X", GUILayout.ExpandWidth(false)))
                {
                    SelectedSubSceneSetting.SubSceneDatList.RemoveAt(i);
                    i --;
                    continue;
                }
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(10f);
                data.center = EditorGUILayout.Vector3Field("Center", data.center);
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(10f);
                data.range = EditorGUILayout.FloatField("Range", data.range);
            }
            EditorGUILayout.EndVertical();

            SelectedSubSceneSetting.SubSceneDatList[i] = data;
        }
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("New SubScne (Exist Scene)"))
            {
                var filePath = EditorUtility.OpenFilePanel("Fine Scene", Application.dataPath, "unity");
                if (string.IsNullOrWhiteSpace(filePath) == false)
                {
                    var sceneName = System.IO.Path.GetFileNameWithoutExtension(filePath);

                    var newSubSceneData = new SubSceneSetting.SubSceneData();
                    newSubSceneData.sceneName = sceneName;
                    SelectedSubSceneSetting.SubSceneDatList.Add(newSubSceneData);
                }
            }
            if (GUILayout.Button("New SubScne (New Scene)"))
            {
                var saveDefaultName = $"{SelectedMainScene.name}_SubScene_{SelectedSubSceneSetting.SubSceneDatList.Count}";
                var filePath = EditorUtility.SaveFilePanelInProject("Create Scene", saveDefaultName, "unity", "message");
                if (string.IsNullOrWhiteSpace(filePath) == false)
                {
                    var sceneName = System.IO.Path.GetFileNameWithoutExtension(filePath);

                    var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
                    newScene.name = sceneName;
                    EditorSceneManager.SaveScene(newScene, filePath);

                    var newSubSceneData = new SubSceneSetting.SubSceneData();
                    newSubSceneData.sceneName = sceneName;
                    SelectedSubSceneSetting.SubSceneDatList.Add(newSubSceneData);

                    var buildSettingSceneList = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
                    buildSettingSceneList.Add(new EditorBuildSettingsScene(filePath, true));
                    EditorBuildSettings.scenes = buildSettingSceneList.ToArray();
                }
            }
        }
        if (GUILayout.Button("Save"))
        {
            EditorUtility.SetDirty(SelectedSubSceneSetting);
            UnityEditor.AssetDatabase.SaveAssets();
        }
    }

    private void AutoSelectMainScene()
    {
        for (int i = 0; i < EditorSceneManager.sceneCount; i ++)
        {
            var sceneData = EditorSceneManager.GetSceneAt(i);
            if (sceneData.IsValid() == false) continue;

            SelectedMainScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneData.path);
            OnChangeSelectedMainScene();
            break;
        }
    }

    private void OnChangeSelectedMainScene()
    {
        if (SelectedMainScene == null)
        {
            SelectedMainScene = null;
            return;
        }

        SelectedSubSceneSetting = AssetDatabase.LoadAssetAtPath<SubSceneSetting>($"Assets/Resources/SubSceneSettings/{SelectedMainScene.name}.asset");
    }
}
