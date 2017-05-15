using System.Collections.Generic;
using System.Linq;
using Algorithms.AStar;
using UnityEngine;
using UnityEngine.UI;
public class TileManager : MonoBehaviour
{
    public static TileManager Instance { get; private set; }
    public Button ShuffleButton, SolveButton;
    public Text ErrorText;
    public Tile[] Tiles;
    public static System.Random R = new System.Random();
    void Awake()
    {
        Instance = this;
        for (int i = 0; i < Tiles.Length; i++)
        {
            Tiles[i].Index = i;
        }
        ErrorText.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        Instance = null;
    }

    public void ShuffleTiles()
    {
        ErrorText.gameObject.SetActive(false);
        var array = Enumerable.Range(0, 9).OrderBy(t => R.Next()).ToArray();
        for (int i = 0; i < array.Length; i++)
        {
            Tiles[i].Value = array[i];
        }
    }

    public void SolvePuzzle()
    {
        ShuffleButton.interactable = false;
        SolveButton.interactable = false;
        ErrorText.gameObject.SetActive(true);
        var startNode = GetNodeFromTiles();
        SetTilesFromNode(startNode);
        var goalNode = new Node
        {
            Tiles = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 0 }
        };
        StartCoroutine(PathFinder.Execute(startNode, goalNode, ErrorText, result =>
        {
            ShuffleButton.interactable = true;
            SolveButton.interactable = true;
            if (result == null)
            {
                ErrorText.gameObject.SetActive(true);
                ErrorText.text = "No Solution";
            }
            else
            {
                ErrorText.gameObject.SetActive(false);
                SetTilesFromNode(result);
            }
        }));

    }

    private Node GetNodeFromTiles()
    {
        int[] startTiles = new int[9];
        for (int i = 0; i < Tiles.Length; i++)
        {
            startTiles[i] = Tiles[i].Value;
        }
        var startNode = new Node
        {
            Tiles = new[] { 8, 6, 7, 2, 5, 4, 3, 0, 1 }
        };
        return startNode;
    }

    private void SetTilesFromNode(Node node)
    {
        for (int i = 0; i < node.Tiles.Length; i++)
        {
            Tiles[i].Value = node.Tiles[i];
        }
    }

    public List<Node> GenerateSuccessors(Node curNode)
    {
        //Debug.Log(curNode.EmptyTileIndex);
        var matchedTile = Tiles.Single(r => r.Match(curNode));
        return matchedTile.GetSuccessors(curNode);
    }
}
