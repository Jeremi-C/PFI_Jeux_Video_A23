using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : TaskBT
{
    GameObject target;
    GameObject follower;
    float speed = 1;
    Vector3 deplacement;
   

    public Follow(GameObject follower,GameObject target,float speed)
    {
        this.follower = follower;
        this.target = target;
        this.speed = speed;

    }

    public override TaskState Execute()
    {
        deplacement = (target.transform.position - follower.transform.position).normalized;
        follower.transform.Translate(deplacement * speed * Time.deltaTime, Space.World);

        follower.transform.LookAt(target.transform.position);
        if ((int)follower.transform.position.y != (int)target.transform.position.y)//si l'objet n'a fini pas son ascension
        {
            return TaskState.Running;
        }
        return TaskState.Success;
    }
}
