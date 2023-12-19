using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BehaviourTree : MonoBehaviour
{
    private Node rootBT;

    void Start()
    {
        Player player = FindObjectOfType<Player>();
        TaskBT[] playerTurnTask =
        {
            new StartCharacterTurn(player),
            new CheckCharacterTurnFinished(player)
        };
        TaskNode playerTurnNode = new TaskNode("playerTurn", playerTurnTask);

        TaskNode ennemiTurnNode = new TaskNode("ennemiTaskNode", new TaskBT[0]);
        TaskBT[] populateEnnemiTask =
        {
            new PopulateEnnemiNode(ennemiTurnNode)
        };//trouve les ennemis dans la map
        TaskNode populateEnnemiNode = new TaskNode("populateEnnemi", populateEnnemiTask);
        Sequence ennemiTurnSequence = new Sequence("ennemiTurnSequence", new Node[] { populateEnnemiNode, ennemiTurnNode });

        TaskNode projectileTurnNode = new TaskNode("projectileTaskNode", new TaskBT[0]);
        TaskBT[] populateProjectileTask =
        {
            new PopulateProjectileNode(projectileTurnNode)
        };//trouve les projectiles dans la map
        TaskNode populateProjectileNode = new TaskNode("populateprojectile", populateProjectileTask);
        Sequence projectileTurnSequence = new Sequence("projectileTurnSequence", new Node[] { populateProjectileNode, projectileTurnNode });

        TaskBT[] endGameAction =
        {
            new CheckCharacterDead(player),
            new CheckEnnemiDead(ennemiTurnNode)
        };
        TaskNode endGameActionNode = new TaskNode("endGameAction", endGameAction);
        rootBT = new Sequence("rootBT", new Node[]{playerTurnNode, ennemiTurnSequence, projectileTurnSequence, endGameActionNode});
   }

    // Update is called once per frame
    void Update()
    {
        rootBT.Evaluate();
    }
    }
