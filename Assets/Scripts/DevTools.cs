using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor( typeof( TextBoxHandler ) ) ]
public class TextScrollerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TextBoxHandler editScript = (TextBoxHandler) target;
        
        
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Send Next Line" ))
        {
            editScript.nextLine();
        } 
    }
}