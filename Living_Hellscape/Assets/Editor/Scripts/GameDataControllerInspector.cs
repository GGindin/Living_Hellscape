using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

[CustomEditor(typeof(GameStorageController))]
public class GameDataControllerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Delete Save Data"))
        {
            GameStorageController dataController = target as GameStorageController;
            dataController.DeleteAllData();
        }
    }
}
