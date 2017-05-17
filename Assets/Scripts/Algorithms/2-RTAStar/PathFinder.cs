using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine;
using Random = System.Random;

namespace Algorithms.RTAStar
{
    public class PathFinder
    {
        public int Cycles;
        public static Random R = new Random();
        public static List<Node> StoredNodes = new List<Node>();
        public static float TotalTime;
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
            Action<Node, int> callback, Action<Node> animateCallback = null, Action<int> statusCallback = null )
        {
            TotalTime = Time.time;
            Cycles = 0;
            if(!StoredNodes.Contains(startNode))
                StoredNodes.Add(startNode);

            Node currentNode = startNode;
            while (true)
            {
                Cycles++;
                if(statusCallback != null)
                    statusCallback(Cycles);

                List<Node> successorNodes = TileManager.Instance.GenerateSuccessors(currentNode);
                Node optimalNode = successorNodes[0];
                float minCost = float.MaxValue;
                float secondMinCost = float.MaxValue;
                foreach (Node successorNode in successorNodes)
                {
                    if (successorNode.HasEqualState(goalNode))
                    {
                        TotalTime = Time.time - TotalTime;
                        callback(successorNode, Cycles);
                        yield break;
                    }
                    successorNode.F = CalculateSuccessorInDepth(depth, goalNode, successorNode, 1);
                    if (successorNode.F <= minCost)
                    {
                        bool assign = true;
                        if (successorNode.F == minCost)
                        {
                            if (R.NextDouble() <= 0.5f)
                                assign = false;
                        }
                        if (assign)
                        {
                            secondMinCost = minCost;
                            minCost = successorNode.F;
                            optimalNode = successorNode;
                        }
                    }
                    else if (successorNode.F > minCost && successorNode.F < secondMinCost)
                    {
                        secondMinCost = successorNode.F;
                    }
                    if (StoredNodes.Any(x => x.HasEqualState(currentNode)))
                    {
                        StoredNodes.Find(x => x.HasEqualState(currentNode)).H = secondMinCost;
                    }
                    else
                    {
                        currentNode.H = minCost;
                        StoredNodes.Add(currentNode);
                    }
                }

                currentNode = optimalNode;
                if (animateCallback != null)
                    animateCallback(currentNode);
                    yield return null;
            }
        }

    }
}

