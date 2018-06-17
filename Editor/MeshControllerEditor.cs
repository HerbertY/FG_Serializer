using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using FGame.CreateMesh;

/// <summary>
/// mesh顶点编辑器  ： 编辑模式下扩展组件          by:heyang   2017/10/9
/// </summary>
[CustomEditor(typeof(MeshVertexManager))]
public class MeshControllerEditor : Editor {
    private MeshVertexManager manager;
    private static string _btnText = "开始编辑顶点";
    private static bool _isStartEditor = false;

    private void OnEnable()
    {
        manager = target as MeshVertexManager;
    }

    public override void OnInspectorGUI()
    {
        //默认垂直，这只防止自己忘记水平的标注
        //EditorGUILayout.BeginVertical();

        //空一排
        EditorGUILayout.Space();
        //Label
        EditorGUILayout.LabelField("顶点编辑器");

        if (GUILayout.Button(_btnText))
        {
            if (manager != null)
            {
                if (_isStartEditor)
                {
                    manager.EndEditor();
                }
                manager.CreateVertex();
                _isStartEditor = true;
            }
        }
        if (_isStartEditor)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("结束编辑"))
            {
                if (manager != null)
                {
                    manager.EndEditor();
                    _isStartEditor = false;
                }
            }
            if (GUILayout.Button("完成编辑"))
            {
                if (manager != null)
                {
                    manager.FinishEditor();
                    _isStartEditor = false;
                }
            }
            GUILayout.EndHorizontal();
        }
    }
}
