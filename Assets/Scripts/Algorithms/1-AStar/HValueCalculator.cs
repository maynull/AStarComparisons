using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Algorithms.AStar
{
    public static class HValueCalculator
    {
        private const float CostFactor = 1.0f;

        public static float Calculate(Node goalNode, Node curNode)
        {
            float result = 0.0f;
            for (int i = 0; i < 9; i++)
            {
                if (goalNode == null) continue;
                int currentNumber = goalNode.Tiles[i];
                int currentIndex = FindTileCurrentIndex(currentNumber, curNode);

                result = GetDistanceToGoalTileForIndex(result, i, currentIndex);
            }

            return result;

        }

        private static float GetDistanceToGoalTileForIndex(float result, int i, int currentIndex)
        {
            if (currentIndex == i + 0)
                return result;

            if (currentIndex == i + 1 || currentIndex == i + 3)
                result += CostFactor;
            else if (currentIndex == i + 2 || currentIndex == i + 4 || currentIndex == i + 6)
                result += 2 * CostFactor;
            else if (currentIndex == i + 5 || currentIndex == i + 7)
                result += 3 * CostFactor;
            else if (currentIndex == i + 8)
                result += 4 * CostFactor;
            return result;
        }

        private static int FindTileCurrentIndex(int goalNumber, Node current)
        {
            for (int j = 0; j < 9; j++)
            {
                if (current.Tiles[j] == goalNumber)
                {
                    return j;
                }
            }

            return -1;
        }
    }
}