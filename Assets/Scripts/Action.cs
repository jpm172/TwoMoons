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

    //just sets availability without calling update action, used for initialization
    public void SetAvailability( bool value )
    {
        isAvailable = value;
    }

    //sets availability and then updates this throughout the game
    public void SetUpdateAvailability( bool value )
    {
        isAvailable = value; 
        GameObject.FindWithTag( "Game Manager" ).GetComponent<GameManager>().UpdateActions();
    }

    public bool IsAvailable => isAvailable;
}
