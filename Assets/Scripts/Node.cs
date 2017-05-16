using System.Linq;

public class Node
{
    public float F;
    public float G;
    public float H;
    public Node Parent;
    public int[] Tiles { get; set; }

    private int _emptyTileIndex;

    public int EmptyTileIndex
    {
        get
        {
            if (_emptyTileIndex == -1)
                _emptyTileIndex = GetEmptyTilePosition(this);

            return _emptyTileIndex;
        }
    }

    public Node()
    {
        _emptyTileIndex = -1;
    }

    public bool HasEqualState(Node node)
    {
        return node != null && Tiles.SequenceEqual(node.Tiles);
    }

    public bool HasEqualEmptyTileIndex(Node node)
    {
        return node != null && node.EmptyTileIndex == EmptyTileIndex;
    }

    private static int GetEmptyTilePosition(Node node)
    {
        int emptyTilePos = -1;

        for (int i = 0; i < 9; i++)
        {
            if (node.Tiles[i] == 0)
            {
                emptyTilePos = i;
                break;
            }
        }

        return emptyTilePos;
    }
}
