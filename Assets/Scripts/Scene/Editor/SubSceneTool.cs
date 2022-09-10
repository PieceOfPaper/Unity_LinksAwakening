using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SubSceneTool : EditorWindow
{
    [MenuItem("Tools/SubScene Tool")]
    public static void Open()
    {
        var window = (SubSceneTool)EditorWindow.GetWindow(typeof(SubSceneTool), false, "SubScene Tool", true);
        window.Show();
    }


    public Vector2 SubSceneScoll = Vector2.zero;

    private void OnGUI()
    {
        //TODO - 메인씬 설정 기능
        //TODO - 서브씬 세팅 파일 생성/변경

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
}
