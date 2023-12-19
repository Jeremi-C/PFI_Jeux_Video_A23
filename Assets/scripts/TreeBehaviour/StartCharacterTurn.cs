using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCharacterTurn : TaskBT
{
    static int i = 0;
    private Character character;

    public StartCharacterTurn(Character character)
    {
        this.character = character;
    }

    public override TaskState Execute()
    {
        if(!character.IsCharacterTurn)
        {
            //Debug.Log(i++ + ""+character.ToString());
            character.Move();
        }

        return TaskState.Success;
    }
}
