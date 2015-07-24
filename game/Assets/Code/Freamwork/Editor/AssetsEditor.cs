using Freamwork;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
[CustomEditor(typeof(Assets))]
public class AssetsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var assetsListSO = serializedObject.FindProperty("assetsList");
        int len = assetsListSO.arraySize;

        Assets assets = (Assets)target;
        var assetsList = assets.assetsList;

        Rect rect = EditorGUILayout.BeginVertical("box");
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("清 空", GUILayout.Width(40f)))
        {
            assetsListSO.ClearArray();
        }
        GUILayout.Label("资源数量:" + len + "    （将资源拖入以添加）");
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < len; i++)
        {
            EditorGUILayout.BeginHorizontal();
            string typeName = assetsList[i] == null ? "Null" : assetsList[i].GetType().Name;
            EditorGUILayout.ObjectField(i + ". " + typeName, assetsList[i], typeof(Object), true);
            if (GUILayout.Button(new GUIContent("×", "移除"), EditorStyles.miniButton, GUILayout.Width(24f)))
            {
                assetsListSO.DeleteArrayElementAtIndex(i);
                if (assetsListSO.arraySize == len)
                {
                    assetsListSO.DeleteArrayElementAtIndex(i);
                    len = assetsListSO.arraySize;
                    i--;
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
        serializedObject.ApplyModifiedProperties();

        if (rect.Contains(Event.current.mousePosition))
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            if (Event.current.type == EventType.DragPerform)
            {
                List<Object> objList = new List<Object>(DragAndDrop.objectReferences);
                foreach (Object obj in assetsList)
                {
                    for (int i = 0; i < objList.Count; i++)
                    {
                        if (obj == objList[i])
                        {
                            EditorUtility.DisplayDialog("提示", "已存在" + obj.name + "！", "确定");
                            objList.RemoveAt(i);
                        }
                        else if (obj.name == objList[i].name &&
                            !EditorUtility.DisplayDialog("提示", "已存在同名资源" + obj.name+"，是否添加？","确定","取消"))
                        {
                            objList.RemoveAt(i);
                        }
                    }
                }
                assetsList.AddRange(objList);
            }
        }
        else
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.None;
        }
    }
}