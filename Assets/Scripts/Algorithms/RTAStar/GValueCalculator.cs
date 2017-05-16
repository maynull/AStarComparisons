using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Algorithms.RTAStar
{
    public static class GValueCalculator
    {

        public const float CostFactor = 0;

        public static float Calculate(Node node)
        {
            return node.G + CostFactor;
        }

    }
}