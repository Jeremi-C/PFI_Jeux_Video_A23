using UnityEngine;

internal class PopulateEnnemiNode : TaskBT
{
    private TaskNode ennemiTaskNode;

    public PopulateEnnemiNode(TaskNode ennemiTaskNode)
    {
        this.ennemiTaskNode = ennemiTaskNode;
    }

    public override TaskState Execute()
    {
        Ennemi[] ennemis = GameObject.FindObjectsOfType<Ennemi>();

        ennemiTaskNode.ClearTasks();
        foreach (Ennemi ennemi in ennemis)
        {
            ennemiTaskNode.AddTask(new StartCharacterTurn(ennemi));
            ennemiTaskNode.AddTask(new CheckCharacterTurnFinished(ennemi));
        }
        return TaskState.Success;
    }
}