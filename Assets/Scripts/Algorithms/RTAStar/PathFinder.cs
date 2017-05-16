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

        static float CalculateSuccessorInDepth(int depth, Node goalNode, Node curNode, List<Node> storedNodes, float costFactor )
        {
            if (depth > 1)
            {
                List<float> finalCost = new List<float>();
                foreach (var successor in TileManager.Instance.GenerateSuccessors(curNode))
                {
                    var cost = CalculateSuccessorInDepth(depth - 1, goalNode, successor, storedNodes, 1 + costFactor);
                    finalCost.Add(cost);
                }
                return finalCost.Min();
            }

            curNode.H = HValueCalculator.Calculate(goalNode, curNode, storedNodes);
            return costFactor + curNode.H;
        }
        public static IEnumerator Execute(Node startNode, Node goalNode, int depth, Text statusText,
            Action<Node> callback, Action<Node> animateCallback = null)
        {
            Cycles = 0;
            var storedNodes = new List<Node> {startNode};
            Node currentNode = startNode;
            while (true)
            {
                Cycles++;
                statusText.text = "Solving " + Cycles;
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
                    successorNode.F =CalculateSuccessorInDepth(depth, goalNode, successorNode, storedNodes, 1);
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
                if (storedNodes.Any(x => x.HasEqualState(currentNode)))
                {
                    var node = storedNodes.Find(x => x.HasEqualState(currentNode));
                    if (node.H < secondMinCost)
                        storedNodes.Find(x => x.HasEqualState(currentNode)).H = secondMinCost;
                }
                else
                {
                    currentNode.H = secondMinCost;
                    storedNodes.Add(currentNode);
                }
                currentNode = optimalNode;
                if (animateCallback != null)
                    animateCallback(currentNode);
                yield return null;
                //yield return new WaitForSeconds(0.5f);
            }
        }

    }
}

