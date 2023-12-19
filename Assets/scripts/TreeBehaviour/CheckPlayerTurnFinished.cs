using UnityEngine;

internal class CheckCharacterTurnFinished : TaskBT
{
    private Character character;

    public CheckCharacterTurnFinished(Character character)
    {
        this.character = character;
    }

    public override TaskState Execute()
    {
        return character.IsCharacterTurn && character != null ? TaskState.Running : TaskState.Success;
    }
} 