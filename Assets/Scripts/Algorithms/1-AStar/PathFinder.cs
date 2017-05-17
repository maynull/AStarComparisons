using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Diagnostics;
using UnityEngine.UI;
using UnityEngine;
namespace Algorithms.AStar
{
    public static class PathFinder
    {
        public static int Cycles { get; set; }
        public static float TotalTime;
        public static IEnumerator Execute(Node startNode, Node goalNode, Text statusText, Action<Node, int> callback)
        {
            TotalTime = Time.time;
            Cycles = 0;

            var openList = new List<Node> { startNode };
            var closedList = new List<Node>();

            while (openList.Count > 0)
            {
                Cycles++;
                statusText.text = "Solving " + Cycles;
                Node currentNode = GetBestNodeFromOpenList(openList);

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                IEnumerable<Node> successorNodes =
                    TileManager.Instance.GenerateSuccessors(currentNode);

                foreach (Node successorNode in successorNodes)
                {
                    if (successorNode.HasEqualState(goalNode))
                    {
                        TotalTime = Time.time - TotalTime;
                        callback(successorNode, Cycles);
                        yield break;
                    }

                    successorNode.G = GValueCalculator.Calculate(currentNode);
                    successorNode.H = HValueCalculator.Calculate(goalNode, successorNode);
                    successorNode.F = successorNode.G + successorNode.H;
                    if (AnyListHasBetterNode(successorNode, openList))
                        continue;

                    openList.Add(successorNode);
                }
                yield return null;
            }
            TotalTime = Time.time - TotalTime;
            callback(null, Cycles);
        }

        private static Node GetBestNodeFromOpenList(IEnumerable<Node> openList)
        {
            return openList.OrderBy(n => n.F).First();
        }

        private static bool AnyListHasBetterNode(Node successor, List<Node> openList)
        {
            var openListHasBetter = openList.FirstOrDefault(n => n.G.Equals(successor.G)
                                                                  && n.F < successor.F) != null;
            return openListHasBetter;
        }

    }
}