using UnityEngine;

internal class PopulateProjectileNode : TaskBT
{
    private TaskNode projectileTurnNode;

    public PopulateProjectileNode(TaskNode projectileTurnNode)
    {
        this.projectileTurnNode = projectileTurnNode;
    }

    public override TaskState Execute()
    {
        Projectile[] projectiles = GameObject.FindObjectsOfType<Projectile>();
        projectileTurnNode.ClearTasks();
        foreach (Projectile projectile in projectiles)
        {
            projectileTurnNode.AddTask(new StartCharacterTurn(projectile));
        }
        foreach (Projectile projectile in projectiles)
        {
            projectileTurnNode.AddTask(new CheckCharacterTurnFinished(projectile));
        }
        return TaskState.Success;
    }
}