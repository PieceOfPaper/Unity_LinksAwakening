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

    private void OnSceneOpened(Scene scene, OpenSceneMode mode)
    {
        if (IsAutoSelectMainScene == true && mode == OpenSceneMode.Single)
            AutoSelectMainScene();
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
        //TODO - 리스트로 보여줌
        //TODO - SubSceneData, 생성, 삭제, 이동
        //TODO - SubSceneData, 씬 등록
        //TODO - SubSceneData, 씬 로드, 언로드, 세이브
        //TODO - SubSceneData, 중심, 거리 계산
    }

    private void AutoSelectMainScene()
    {
        for (int i = 0; i < UnityEditor.SceneManagement.EditorSceneManager.sceneCount; i ++)
        {
            var sceneData = UnityEditor.SceneManagement.EditorSceneManager.GetSceneAt(i);
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
