using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Algorithms.AStar
{
    public static class GValueCalculator
    {

        public const float CostFactor = 0.265f;

        public static float Calculate(Node node)
        {
            return node.G + CostFactor;
        }

    }
}