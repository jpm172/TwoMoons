using System;
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

[CustomEditor( typeof( TaskWindow ) ) ]
public class TaskWindowEditor : Editor
{
    private string test = "1";
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TaskWindow editScript = (TaskWindow) target;
        
        test = GUILayout.TextField(test, 3, GUILayout.Width(50));
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Complete Task" ))
        {
            //editScript.TaskCompleted( int.Parse( test ) );
        } 
        
    }
}

[CustomEditor( typeof( MoonTrackerGame ) ) ]
public class MoonTrackerGameEditor : Editor
{
    public override void OnInspectorGUI()
    {
        
        DrawDefaultInspector();

        MoonTrackerGame editScript = (MoonTrackerGame) target;
        
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Start Game" ))
        {
            editScript.StartAction();
        } 
        
        
    }
}

[CustomEditor( typeof( AnomalyFinderGame ) ) ]
public class AnomalyFinderGameEditor : Editor
{
    public override void OnInspectorGUI()
    {
        
        DrawDefaultInspector();

        AnomalyFinderGame editScript = (AnomalyFinderGame) target;
        
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Start Game" ))
        {
            editScript.StartAction();
        } 
        
        
    }
}

[CustomEditor( typeof( Settings ) ) ]
public class SettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        
        DrawDefaultInspector();

        Settings editScript = (Settings) target;
        
        //GUI.backgroundColor = new Color(255, 172, 28);
        GUI.backgroundColor = new Color(1, .674f, .1f);
        if (GUILayout.Button("Save Settings",GUILayout.Width(100) ))
        {
            editScript.SaveSettings();
        } 
        
        GUI.backgroundColor = Color.blue;
        if (GUILayout.Button("Load Settings",GUILayout.Width(100) ))
        {
            editScript.LoadSettings();
        } 
        
        
    }
}