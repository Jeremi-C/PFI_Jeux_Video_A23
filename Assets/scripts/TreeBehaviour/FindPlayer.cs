using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class FindPlayer : TaskBT
{
    GameObject finder;//celui qui chercher le joueur
    const int rayWidth = 5;
    Vector3[] rayArray = new Vector3[] {
            Vector3.down,
            new Vector3(rayWidth, -100, 0),
            new Vector3(-rayWidth, -100, 0),
            new Vector3(0, -100, rayWidth),
            new Vector3(0,-100,-rayWidth),

            new Vector3(rayWidth, -100, rayWidth),
            new Vector3(-rayWidth, -100, rayWidth),
            new Vector3(rayWidth, -100, -rayWidth),
            new Vector3(-rayWidth, -100, -rayWidth)
        };

    public FindPlayer(GameObject finder)
    {
        this.finder = finder;
    }

    public override TaskState Execute()
    {
        foreach (Vector3 ray in rayArray)
        {
            Debug.DrawRay(finder.transform.position, finder.transform.TransformDirection(ray) * 1000, Color.blue);
            if (Physics.Raycast(finder.transform.position, finder.transform.TransformDirection(ray), out RaycastHit hit, 1000) && hit.collider.CompareTag("Player"))
            {
                Debug.Log("le joueur a été touché");
                return TaskState.Success;
            }
        }
        return TaskState.Running;
    }
}
