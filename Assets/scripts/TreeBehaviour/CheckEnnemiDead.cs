using TMPro;
using UnityEngine;

internal class CheckEnnemiDead : TaskBT
{
    private TaskNode ennemiTurnNode;

    public CheckEnnemiDead(TaskNode ennemiTurnNode)
    {
        this.ennemiTurnNode = ennemiTurnNode;
    }

    public override TaskState Execute()
    {
        if(ennemiTurnNode.Count== 0)
        {
            var endGameTextUi = GameObject.Find("EndGameText (TMP)").GetComponent<TextMeshProUGUI>();
            endGameTextUi.text = "Tu as gagné";
        }
        return TaskState.Success;
    }
}