using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Algorithms.RTAStar
{
    public class PathFinder
    {
        public static int Cycles;
        public static System.Random R = new System.Random();
        public static List<Node> StoredNodes = new List<Node>();

        static float CalculateSuccessorInDepth(int depth, Node goalNode, Node curNode, float costFactor )
        {
            if (depth > 1)
            {
                List<float> finalCost = new List<float>();
                foreach (var successor in TileManager.Instance.GenerateSuccessors(curNode))
                {
                    var cost = CalculateSuccessorInDepth(depth - 1, goalNode, successor, 1 + costFactor);
                    finalCost.Add(cost);
                }
                return finalCost.Min();
            }

            curNode.H = HValueCalculator.Calculate(goalNode, curNode, StoredNodes);
            return costFactor + curNode.H;
        }
        public IEnumerator Execute(Node startNode, Node goalNode, int depth,
            Action<Node> callback, Action<Node> animateCallback = null, Action<string> statusCallback = null )
        {
            Cycles = 0;
            if(!StoredNodes.Contains(startNode))
                StoredNodes.Add(startNode);
            //StoredNodes = new List<Node> {startNode};
            Node currentNode = startNode;
            while (true)
            {
                Cycles++;
                if(statusCallback != null)
                    statusCallback("Solving " + Cycles);
                List<Node> successorNodes = TileManager.Instance.GenerateSuccessors(currentNode);
                Node optimalNode = successorNodes[0];
                float minCost = float.MaxValue;
                float secondMinCost = float.MaxValue;
                foreach (Node successorNode in successorNodes)
                {
                    if (successorNode.HasEqualState(goalNode))
                    {
                        callback(successorNode);
                        yield break;
                    }
                    successorNode.F =CalculateSuccessorInDepth(depth, goalNode, successorNode, 1);
                    if (successorNode.F <= minCost)
                    {
                        secondMinCost = minCost;
                        minCost = successorNode.F;
                        optimalNode = successorNode;
                    }
                    else if (successorNode.F > minCost && successorNode.F < secondMinCost)
                    {
                        secondMinCost = successorNode.F;
                    }
                }
                if (StoredNodes.Any(x => x.HasEqualState(currentNode)))
                {
                    var node = StoredNodes.Find(x => x.HasEqualState(currentNode));
                    if (node.H < secondMinCost)
                        StoredNodes.Find(x => x.HasEqualState(currentNode)).H = secondMinCost;
                }
                else
                {
                    currentNode.H = minCost;
                    StoredNodes.Add(currentNode);
                }
                currentNode = optimalNode;
                if (animateCallback != null)
                    animateCallback(currentNode);
                yield return null;
            }
        }

    }
}

