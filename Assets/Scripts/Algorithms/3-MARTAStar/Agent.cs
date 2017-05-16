using System;
using System.Collections;
using System.Collections.Generic;
using Algorithms.RTAStar;
using UnityEngine;

public class Agent
{
    readonly PathFinder finder = new PathFinder();

    public IEnumerator AgentStep(Node startNode, Node goalNode, int depth, Action<Node> finalCallback, Action<Node> animateCallback, Action<string> statusCallback)
    {
        yield return finder.Execute(startNode, goalNode, depth, finalCallback, animateCallback, statusCallback);
    }

}
