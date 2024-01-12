using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Task
{
    [SerializeField]private string taskName;
    [SerializeField]private int taskID;
    [SerializeField] private bool isCompleted;
    public Task( string name, int ID )
    {
        taskName = name;
        taskID = ID;
    }

    public string TaskName
    {
        get => taskName;
        set => taskName = value;
    }

    public int TaskID
    {
        get => taskID;
    }

    public bool IsCompleted
    {
        get => isCompleted;
        set => isCompleted = value;
    }
}
