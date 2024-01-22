using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepAction : Action
{
    public override void StartAction()
    {
        Debug.Log( "sleep action" );
        GameManager game = GetComponent<GameManager>();

        foreach ( Action a in game.Tasks )
        {
            if ( a.IsAvailable )
            {
                return;
            }
        }
        
        game.StartNewDay();
    }
}
