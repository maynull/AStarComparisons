using System;
using System.Collections;
using System.Collections.Generic;
using Algorithms.RTAStar;
using UnityEngine;

public class Agent
{
    readonly PathFinder finder = new PathFinder();

    public IEnumerator AgentStep(int i, Node startNode, Node goalNode, int depth, Action<Node, int> finalCallback, Action<Node> animateCallback, Action<int> statusCallback)
    {
        yield return new WaitForSeconds(i/3.0f);
        yield return finder.Execute(startNode, goalNode, depth, finalCallback, animateCallback, statusCallback);
    }

}
