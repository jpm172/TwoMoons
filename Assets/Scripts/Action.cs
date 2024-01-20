using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action : MonoBehaviour
{
    public String actionName;
    public String actionDescription;
    private bool isAvailable;

    public virtual void StartAction()
    {
    }

    public virtual void CompleteAction()
    {
    }
    
    public bool IsAvailable
    {
        get => isAvailable;
        set
        {
            isAvailable = value; 
            GameObject.FindWithTag( "Game Manager" ).GetComponent<GameManager>().UpdateActions();
        }
    }
}
