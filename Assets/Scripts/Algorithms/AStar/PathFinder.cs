using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine.UI;
namespace Algorithms.AStar
{
    public static class PathFinder
    {
        public static int Cycles { get; set; }
        public static IEnumerator Execute(Node startNode, Node goalNode, Text errorText, Action<Node> callback)
        {
            Cycles = 0;

            var openList = new List<Node> { startNode };
            var closedList = new List<Node>();

            while (openList.Count > 0)
            {
                Cycles++;
                errorText.text = "Solving " + Cycles;
                Node currentNode = GetBestNodeFromOpenList(openList);

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                IEnumerable<Node> successorNodes =
                    TileManager.Instance.GenerateSuccessors(currentNode);

                foreach (Node successorNode in successorNodes)
                {
                    if (successorNode.HasEqualState(goalNode))
                    {
                        callback(successorNode);
                        yield break;
                    }

                    successorNode.G = GValueCalculator.Calculate(currentNode);
                    successorNode.H = HValueCalculator.Calculate(goalNode, successorNode);
                    successorNode.F = successorNode.G + successorNode.H;

                    if (AnyListHasBetterNode(successorNode, openList, closedList))
                        continue;

                    openList.Add(successorNode);
                }
                
                yield return null;
            }

            callback(null);
        }

        private static Node GetBestNodeFromOpenList(IEnumerable<Node> openList)
        {
            return openList.OrderBy(n => n.F).First();
        }

        private static bool AnyListHasBetterNode(Node successor, List<Node> openList, List<Node> closedList)
        {
            var openListHasBetter = openList.FirstOrDefault(n => n.G.Equals(successor.G)
                                                                  && n.F < successor.F) != null;
            //var closedListHasBetter = closedList.FirstOrDefault(n => n.G.Equals(successor.G)
            //                                                          && n.F < successor.F) != null;
            return openListHasBetter;// || closedListHasBetter;
        }

    }
}