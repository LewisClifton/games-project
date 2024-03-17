using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// provide the component type for which this inspector UI is required
[CustomEditor(typeof(Gameplay))]
public class MMInspector : Editor
{
    public override void OnInspectorGUI()
    {
        // will enable the default inpector UI 
        base.OnInspectorGUI();

        // implement your UI code here
        Gameplay gameplay = (Gameplay)target;

        //Button
        if (GUILayout.Button("Extract data from animator"))
        {
            gameplay.ExtractData();
        }
    }
}