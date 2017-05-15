using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class SuccessorCalculationRule
{
    public virtual bool Match(Node node)
    {
        return false;
    }

    public static void AddSuccessor(Node node, ICollection<Node> result, int swapTile)
    {
        var newState = node.Tiles.Clone() as int[];
        if (newState == null) return;
        newState[node.EmptyTileIndex] = newState[swapTile];
        newState[swapTile] = 0;

        if (!IsEqualToParentState(node.Parent, newState))
            result.Add(new Node { Tiles = newState, Parent = node });
    }

    private static bool IsEqualToParentState(Node node, int[] state)
    {
        return node != null && state.SequenceEqual(node.Tiles);
    }
}
