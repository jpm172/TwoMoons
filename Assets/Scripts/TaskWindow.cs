using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TaskWindow : MonoBehaviour
{
    public GameObject taskContainer;
    public List<Task> taskList;
    
    private TextMeshProUGUI[] textSlots;
    // Start is called before the first frame update
    void Start()
    {
        textSlots = taskContainer.GetComponentsInChildren<TextMeshProUGUI>();
    }

    public void initializeTasks()
    {
        Debug.Log( "Init tasks" );
        ResetText();
        
        
        taskList = new List<Task>();
        taskList.Add( new Task( "Task 1", 1 ) );
        taskList.Add( new Task( "Task 2", 2 ) );
        taskList.Add( new Task( "Task 3", 3 ) );
        taskList.Add( new Task( "Task 4", 4 ) );
        
        UpdateText();
        
    }

    private void ResetText()
    {
        foreach ( TextMeshProUGUI tmp in textSlots )
        {
            tmp.text = "";
            tmp.alpha = 1;
            tmp.fontStyle = FontStyles.Normal;
        }
    }

    private void UpdateText()
    {
        for ( int i = 0; i < taskList.Count; i++ )
        {
            textSlots[i].text = taskList[i].TaskName;
        }
    }
    
    public void TaskCompleted(int ID)
    {
        bool success = false;
        for ( int i = 0; i < taskList.Count; i++ )
        {
            if ( taskList[i].TaskID == ID )
            {
                taskList[i].IsCompleted = true;
                textSlots[i].fontStyle = FontStyles.Strikethrough;
                textSlots[i].alpha = .5f;
                success = true;
            }
        }

        if ( !success )
        {
            Debug.LogError( "Task " + ID + " is invalid" );
        }

        //TODO: play scribbling-out noise
    }
    
    
}
