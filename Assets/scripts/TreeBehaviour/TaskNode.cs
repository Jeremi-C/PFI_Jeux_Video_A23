using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum TaskState { Running, Success, Failure }
public abstract class TaskBT 
{
    public abstract TaskState Execute();
}

public class TaskNode : Node
{
    protected List<TaskBT> Tasks { get; private set; } = new();
    private int CurrentTaskIndex { get; set; }
    public int Count
    {
        get { return Tasks.Count; }
    }

    public TaskNode(string tag, IEnumerable<TaskBT> tasks)
        : base(tag)
    {
        foreach (TaskBT task in tasks)
        {
            AddTask(task);
        }
    }

    public void AddTask(TaskBT task) => Tasks.Add(task);

    public void ClearTasks()
    {
        Tasks.Clear();
    }
    protected override NodeState InnerEvaluate()
    {
        bool executeNextTask = true;
        int taskCount = Tasks.Count;
        if (Tasks.Count == 0)
        {
            return NodeState.Success;
        }
        while (executeNextTask)
        {
            TaskBT currentTask = Tasks[CurrentTaskIndex];
            TaskState currentTaskState = currentTask.Execute();

            switch (currentTaskState)
            {
                case TaskState.Failure:
                    State = NodeState.Failure;
                    return State;
                case TaskState.Running:
                    executeNextTask = false;
                    break;
                case TaskState.Success:
                    if (CurrentTaskIndex == taskCount - 1)
                    {
                        CurrentTaskIndex = 0;
                        State = NodeState.Success;
                        return State;
                    }
                  
                    ++CurrentTaskIndex;
                    break;
            }
        }

        State = NodeState.Running;
        return State;
    }
}

