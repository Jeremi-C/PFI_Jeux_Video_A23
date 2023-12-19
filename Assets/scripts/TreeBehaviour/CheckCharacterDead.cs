using System;
using TMPro;
using UnityEngine;

internal class CheckCharacterDead : TaskBT
{
    private Player player;

    public CheckCharacterDead(Player player)
    {
        this.player = player;
    }

    public override TaskState Execute()
    {
        if (player.IsDead)
        {
            var endGameTextUi = GameObject.Find("EndGameText (TMP)").GetComponent<TextMeshProUGUI>();
            endGameTextUi.text = "Tu as perdu";
        }
        return TaskState.Success;
    }
}